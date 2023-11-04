using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartMonitoring.Server.Entities;

/// <summary>
/// Сущность приглашений.
/// </summary>
[Index(nameof(Code), IsUnique = true)]
public class InviteEntity
{
    /// <summary>
    /// ID.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Количество разрешённого использования.
    /// </summary>
    public int UseCount { get; set; }
    
    /// <summary>
    /// Количество использований.
    /// </summary>
    public int UsedCount { get; set; }
    
    /// <summary>
    /// Код.
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// Дата время создания.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Дата время окончания использования.
    /// </summary>
    public DateTime ExpireAt { get; set; } = DateTime.Now.AddDays(1);
    
    /// <summary>
    /// ID организации.
    /// </summary>
    public Guid OrganizationID { get; set; }
    
    /// <summary>
    /// Сущность организации.
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public OrganizationEntity? Organization { get; set; }
}