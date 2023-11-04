using Microsoft.AspNetCore.SignalR.Client;
using Serilog;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Client.HubClients;

public class LogHubClient
{
    public const string HubURI = "/hubs/logs";

    private readonly HubConnection Hub;

    public delegate Task OnAdd(LogViewModel log);
    public delegate Task OnAddID(Guid id);

    public event OnAdd OnAddEvent;
    public event OnAddID OnAddIDEvent;

    public LogHubClient(Uri uri)
    {
        Hub = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl(new Uri(uri, HubURI))
            .Build();

        Hub.Closed += OnClosed;
        Hub.Reconnected += OnReconnected;
        Hub.Reconnecting += OnReconnecting;

        Hub.On("Add", async (LogViewModel log) => await OnAddEvent(log));
        Hub.On("AddID", async (Guid id) => await OnAddIDEvent(id));

        StartConnection();
    }

    private async Task OnClosed(Exception exception)
    {
        Log.Information($"[{DateTime.Now.ToString()}] WebSocket die!");
    }

    private async Task OnReconnected(string connectionId)
    {
        Log.Information($"[{DateTime.Now.ToString()}] WebSocket reconnected!");
    }

    private async Task OnReconnecting(Exception exception)
    {
        Log.Information($"[{DateTime.Now.ToString()}] WebSocket reconnecting...");
    }

    public void StartConnection()
    {
        Task.Run(async () => await StartConnectionAsync());
    }

    public async Task StartConnectionAsync()
    {
        await Hub.StartAsync();
        Log.Verbose(Hub.State.ToString("G"));
    }
}