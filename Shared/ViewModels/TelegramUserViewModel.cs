namespace SmartMonitoring.Shared.ViewModels;

public class TelegramUserViewModel
{
    public int TelegramID { get; set; }
    public bool NotificationStatus { get; set; } = true;
    public string? MetaInfo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastUseAt { get; set; } = DateTime.Now;
    public Guid OrganizationID { get; set; }
}