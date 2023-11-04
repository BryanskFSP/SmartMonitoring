using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using SmartMonitoring.Server;
using SmartMonitoring.Server.Hubs;
using SmartMonitoring.Server.Jobs;
using SmartMonitoring.Server.Providers;
using SmartMonitoring.Server.Services;

// Initialize logger.
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Server are starting!");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
    builder.Host.UseSerilog();
    
    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
    
    // Initialize DB Context.

    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                           "Host=90.156.208.88;Database=bruanskbd1;Username=bruansk;Password=bruansk";
    Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", connectionString);

    builder.Services.AddDbContext<SMContext>(options =>
        options.UseNpgsql(connectionString));

    // Initialize Redis.
    
    builder.Services.AddStackExchangeRedisCache(options => {
        options.Configuration = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
        options.InstanceName = "local";
    });
    
    // Add services to the container.

    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    #region Services

    builder.Services.AddScoped<AuthService>();
    builder.Services.AddTransient<AdminService>();
    builder.Services.AddTransient<DataBaseService>();
    builder.Services.AddTransient<InviteService>();
    builder.Services.AddTransient<LogService>();
    builder.Services.AddTransient<OrganizationService>();
    builder.Services.AddTransient<PSQLService>();
    builder.Services.AddTransient<PSQLCheckerService>();
    builder.Services.AddTransient<TelegramUserService>();
    builder.Services.AddTransient<WikiService>();
    builder.Services.AddTransient<WikiSolutionService>();

    builder.Services.AddAutoMapper(typeof(Mappings));

    var botUrlStr = Environment.GetEnvironmentVariable("BOT_URL") ?? "http://localhost:3000";
    RefitProvider.AddRefitInterfaces(builder.Services, new Uri(botUrlStr));

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

// JSON settings.

    builder.Services.AddControllersWithViews().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition
            = JsonIgnoreCondition.WhenWritingNull;
    });

    builder.Services.AddResponseCompression(opts =>
    {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" });
    });
    
    builder.Services.AddQuartz(q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();
    });
    builder.Services.AddQuartzHostedService(opt =>
    {
        opt.WaitForJobsToComplete = true;
    });
    
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
    
    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");

    app.MapHub<LogHub>(LogHub.HubURI);
    app.UseResponseCompression();
    
    var scope = app.Services.CreateScope().ServiceProvider;
    var db = scope.GetService<SMContext>();
    await db.Database.EnsureCreatedAsync();

    
    if (!string.IsNullOrWhiteSpace(botUrlStr))
    {
        var schedulerFactory = scope.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<CheckerJob>()
            .WithIdentity("myJob", "group1")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("myTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(15)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(job, trigger);
        
        // await scope.GetService<CheckerService>().Start();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
    throw;
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}