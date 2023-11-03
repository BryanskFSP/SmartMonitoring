using System.Reflection;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SmartMonitoring.Server;
using SmartMonitoring.Server.Hubs;
using SmartMonitoring.Server.Jobs;
using SmartMonitoring.Server.Providers;
using SmartMonitoring.Server.Services;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
// Initialize DB Context.

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                       "Host=90.156.208.88;Database=bruanskbd1;Username=bruansk;Password=bruansk";
Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", connectionString);

builder.Services.AddDbContext<SMContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

#region Services

builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<DataBaseService>();
builder.Services.AddScoped<InviteService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<PSQLService>();
builder.Services.AddScoped<TelegramUserService>();

builder.Services.AddAutoMapper(typeof(Mappings));

var botUrlStr = Environment.GetEnvironmentVariable("BOT_URL") ?? "http://localhost:3000";
RefitProvider.AddRefitInterfaces(builder.Services, new Uri(botUrlStr));


builder.Services.AddTransient<CheckerService>();
builder.Services.AddTransient<JobFactory>(o => new JobFactory(builder.Services.BuildServiceProvider()));
builder.Services.AddTransient<CheckerJob>();

#endregion

builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Smart Monitoring API",
            Description = "An ASP.NET Core Web API for Smart Monitoring",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Example Contact",
                Url = new Uri("https://example.com/contact")
            },
            License = new OpenApiLicense
            {
                Name = "Example License",
                Url = new Uri("https://example.com/license")
            }
        });

        // using System.Reflection;
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

var scope = app.Services.CreateScope().ServiceProvider;
var db = scope.GetService<SMContext>();
await db.Database.EnsureCreatedAsync();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.MapHub<LogHub>(LogHub.HubURI);

if (!string.IsNullOrWhiteSpace(botUrlStr))
{
    await scope.GetService<CheckerService>().Start();
}

app.Run();