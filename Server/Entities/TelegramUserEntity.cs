using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartMonitoring.Server.Entities;

/// <summary>
/// Сущность Telegram пользователей.
/// </summary>
[Index(nameof(TelegramID), IsUnique = true)]
public class TelegramUserEntity
{
    /// <summary>
    /// ID пользователя в Telegram.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long TelegramID { get; set; }

    public bool NotificationStatus { get; set; } = true;

    public string? MetaInfo { get; set; }

    /// <summary>
    /// Дата время создания.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Дата время последнего пользования.
    /// </summary>
    public DateTime LastUseAt { get; set; } = DateTime.Now;
    
    
    public Guid OrganizationID { get; set; }
    
    [ForeignKey(nameof(OrganizationID))]
    public OrganizationEntity? Organization { get; set; }
}