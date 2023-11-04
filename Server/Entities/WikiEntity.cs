using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Entities;

/// <summary>
/// Сущность Wiki.
/// </summary>
[Index(nameof(Type), IsUnique = true)]
public class WikiEntity
{
    /// <summary>
    /// ID.
    /// </summary>
    public Guid ID { get; set; }
    
    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Тип ошибки.
    /// </summary>
    public ActionType Type { get; set; }

    public HashSet<WikiSolutionEntity>? WikiSolutions { get; set; } = new();
}

/// <summary>
/// Сущеность решения проблемы.
/// </summary>
public class WikiSolutionEntity
{
    /// <summary>
    /// ID.
    /// </summary>
    public Guid ID { get; set; }
    
    /// <summary>
    /// Wiki ID.
    /// </summary>
    public Guid WikiID { get; set; }
    
    /// <summary>
    /// Сущность Wiki.
    /// </summary>
    [ForeignKey(nameof(WikiID))]
    public WikiEntity? Wiki { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public string SqlScript { get; set; }
    public Guid? OrganizationID { get; set; }
    [ForeignKey(nameof(OrganizationID))]
    public OrganizationEntity? Organization { get; set; }
}