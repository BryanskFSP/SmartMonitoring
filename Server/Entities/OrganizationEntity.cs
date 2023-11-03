namespace SmartMonitoring.Server.Entities;

public class OrganizationEntity
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    
    public HashSet<AdminEntity>? Admins { get; set; } = new();
    public HashSet<DataBaseEntity>? DataBases { get; set; } = new();
    public HashSet<InviteEntity>? Invites { get; set; } = new();
    public HashSet<LogEntity>? Logs { get; set; } = new();
    public HashSet<TelegramUserEntity>? TelegramUsers { get; set; } = new();
}