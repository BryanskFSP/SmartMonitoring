using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMonitoring.Server.Entities;

public class DataBaseEntity
{
    public Guid ID { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string Host { get; set; }
    public string Database { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    
    public Guid OrganizationID { get; set; }
    
    [ForeignKey(nameof(OrganizationID))]
    public OrganizationEntity? Organization { get; set; }
}