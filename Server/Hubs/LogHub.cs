using Microsoft.AspNetCore.SignalR;

namespace SmartMonitoring.Server.Hubs;

public class LogHub: Hub
{
    public const string HubURI = "/hubs/logs";

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Connected {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnected {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
}