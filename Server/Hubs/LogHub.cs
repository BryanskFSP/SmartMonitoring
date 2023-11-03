using Microsoft.AspNetCore.SignalR;

namespace SmartMonitoring.Server.Hubs;

public class LogHub: Hub
{
    public const string HubURI = "/hubs/logs";
}