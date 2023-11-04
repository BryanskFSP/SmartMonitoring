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

    /// <summary>
    /// ID администратора.
    /// </summary>
    public Guid? AdminID { get; set; }
    
    /// <summary>
    /// Сущность Администратора.
    /// </summary>
    [ForeignKey(nameof(AdminID))]
    public AdminEntity? Admin { get; set; }

    /// <summary>
    /// Статус уведомлений.
    /// </summary>
    public bool NotificationStatus { get; set; } = true;

    /// <summary>
    /// Meta info.
    /// </summary>
    public string? MetaInfo { get; set; }

    /// <summary>
    /// Дата время создания.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Дата время последнего пользования.
    /// </summary>
    public DateTime LastUseAt { get; set; } = DateTime.Now;
    
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