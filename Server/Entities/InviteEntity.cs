using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartMonitoring.Server.Entities;

[Index(nameof(Code), IsUnique = true)]
public class InviteEntity
{
    public Guid ID { get; set; }
    
    public string Code { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ExpireAt { get; set; } = DateTime.Now.AddDays(1);
    
    public Guid OrganizationID { get; set; }
    
    [ForeignKey(nameof(OrganizationID))]
    public OrganizationEntity? Organization { get; set; }
}