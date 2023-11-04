namespace SmartMonitoring.Shared.ViewModels;

public class InviteViewModel
{
    public Guid ID { get; set; }
    
    public string Code { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ExpireAt { get; set; } = DateTime.Now.AddDays(1);
    
    public Guid OrganizationID { get; set; }
    public OrganizationViewModel? Organization { get; set; }
}