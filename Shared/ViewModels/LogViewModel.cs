using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Shared.ViewModels;

public class LogViewModel
{
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? OrganizationID { get; set; }
    public OrganizationViewModel? Organization { get; set; }
    public Guid? DataBaseID { get; set; }
    public DataBaseViewModel? DataBase { get; set; }
    public ActionType Action { get; set; }
    public LogType LogType { get; set; }
    public string? Entity { get; set; }
    public string? EntityID { get; set; }
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