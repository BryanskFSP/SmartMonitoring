using Microsoft.AspNetCore.SignalR;
using Serilog;

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
        Log.Information($"{Context.ConnectionId} connected {DateTime.Now}");
        return base.OnConnectedAsync();
    }

    /// <summary>
    /// On disconnected async handler.
    /// </summary>
    /// <param name="exception">Exception.</param>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Log.Information(exception, $"{Context.ConnectionId} disconnected {DateTime.Now}");
        return base.OnDisconnectedAsync(exception);
    }
}