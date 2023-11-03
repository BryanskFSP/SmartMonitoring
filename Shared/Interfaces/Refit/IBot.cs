using Refit;

namespace SmartMonitoring.Shared.Interfaces.Refit;

public interface IBot
{
    [Get("/api/status")]
    Task<string> GetStatus();

    [Post("/api/users/message/send")]
    Task<string> SendMessage([Body] string text);
    
    [Post("/api/org/{orgId}/message/send")]
    Task<string> SendMessageInOrg(Guid orgId, [Body] string text, Guid? logId = null);

    [Post("/api/users/{userId}/message/send")]
    Task<string> SendMessageInUser(long userId, string text, Guid? logId = null);
    //
    // [Post("message/send")]
    // Task SendMessage(BotMessageSendModel model);
}

public class BotMessageSendModel
{
    public List<int> IDs { get; set; }
    public string Message { get; set; }
}