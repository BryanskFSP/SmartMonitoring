using SmartMonitoring.Shared.Models;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared.EditModels;

public class WikiEditModel
{
    public string Name { get; set; }
    public ActionType Type { get; set; }
    public List<WikiSolutionEditModel>? WikiSolutions { get; set; } = new();
}

public class WikiSolutionEditModel
{
    public Guid WikiID { get; set; } = Guid.Empty;
    public string Name { get; set; }
    public string Description { get; set; }
    public string SqlScript { get; set; }
    
    public Guid? OrganizationID { get; set; }
    public OrganizationViewModel? Organization { get; set; }
}
