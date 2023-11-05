using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartMonitoring.Client;
using MudBlazor.Services;
using Serilog;
using Serilog.Core;
using SmartMonitoring.Client.Providers;

// Initialize logger.
var levelSwitch = new LoggingLevelSwitch();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .WriteTo.BrowserConsole()
    .CreateLogger();

Log.Information("Client are starting!");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

RefitProvider.AddRefitInterfaces(builder.Services, new Uri(builder.HostEnvironment.BaseAddress));

await builder.Build().RunAsync();