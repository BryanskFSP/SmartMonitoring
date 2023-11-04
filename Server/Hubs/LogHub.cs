using Microsoft.AspNetCore.SignalR;

namespace SmartMonitoring.Server.Hubs;

public class LogHub: Hub
{
    public const string HubURI = "/hubs/logs";

    // <summary>
    /// On connected async handler.
    /// </summary>
    /// <returns></returns>
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected {DateTime.Now}");
        return base.OnConnectedAsync();
    }

    /// <summary>
    /// On disconnected async handler.
    /// </summary>
    /// <param name="exception"></param>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"{Context.ConnectionId} disconnected {DateTime.Now}");
        return base.OnDisconnectedAsync(exception);
    }
}