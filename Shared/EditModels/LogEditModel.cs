using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Shared.EditModels;

public class LogEditModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? OrganizationID { get; set; }
    public Guid? DataBaseID { get; set; }
    public ActionType Action { get; set; }
    public LogType LogType { get; set; }
    public string? Entity { get; set; }
    public string? EntityID { get; set; }
    public string? EntityJSON { get; set; }
}