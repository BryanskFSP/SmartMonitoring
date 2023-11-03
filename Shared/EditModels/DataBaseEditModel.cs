
namespace SmartMonitoring.Shared.EditModels;

public class DataBaseEditModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Host { get; set; }
    public string User { get; set; }
    public string Database { get; set; }
    public string Password { get; set; }
    public Guid OrganizationID { get; set; }
}