
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMonitoring.Server.Entities;

public class AdminEntity
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public Guid OrganizationID { get; set; }
    
    [ForeignKey(nameof(OrganizationID))]
    public OrganizationEntity? Organization { get; set; }

    public string Login { get; set; }
    public string PasswordHash { get; set; }
}