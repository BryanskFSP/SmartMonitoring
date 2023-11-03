namespace SmartMonitoring.Shared.EditModels;

public class TelegramUserEditModel
{
    public int TelegramID { get; set; }
    public bool NotificationStatus { get; set; } = true;
    public Guid? OrganizationID { get; set; }
    public string? MetaInfo { get; set; }
}