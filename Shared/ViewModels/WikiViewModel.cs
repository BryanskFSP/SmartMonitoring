using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Shared.ViewModels;

public class WikiViewModel
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public ActionType Type { get; set; }
    public List<WikiSolutionViewModel>? WikiSolutions { get; set; } = new();
}

public class WikiSolutionViewModel
{
    public Guid ID { get; set; }
    public Guid WikiID { get; set; }
    public WikiViewModel? Wiki { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string SqlScript { get; set; }
    
    public Guid? OrganizationID { get; set; }
    public OrganizationViewModel? Organization { get; set; }
}