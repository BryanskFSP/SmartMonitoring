using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Entities;

/// <summary>
/// Сущность логов.
/// </summary>
public class LogEntity
{
    /// <summary>
    /// ID.
    /// </summary>
    [Key]
    public Guid ID { get; set; }
    
    /// <summary>
    /// Название.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// ID организации.
    /// </summary>
    public Guid? OrganizationID { get; set; }
    
    /// <summary>
    /// Сущность организации.
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public OrganizationEntity? Organization { get; set; }

    /// <summary>
    /// ID базы данных.
    /// </summary>
    public Guid? DataBaseID { get; set; }
    
    /// <summary>
    /// Сущность базы данных.
    /// </summary>
    public DataBaseEntity? DataBase { get; set; }

    /// <summary>
    /// Тип действия.
    /// </summary>
    public ActionType Action { get; set; }
    
    /// <summary>
    /// Тип лога.
    /// </summary>
    public LogType LogType { get; set; }
    
    /// <summary>
    /// Сущность.
    /// </summary>
    public string? Entity { get; set; }
    
    /// <summary>
    /// ID cущности.
    /// </summary>
    public string? EntityID { get; set; }
    
    /// <summary>
    /// JSON сущности.
    /// </summary>
    public string? EntityJSON { get; set; }

    /// <summary>
    /// Дата и время добавления.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Статус исправления.
    /// </summary>
    public bool FixStatus { get; set; } = false;
    
    /// <summary>
    /// Дата и время обновления.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}