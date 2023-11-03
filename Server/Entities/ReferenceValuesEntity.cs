using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMonitoring.Server.Entities;

public class ReferenceValueEntity
{
    public Guid ID { get; set; }
    public ReferenceType Type { get; set; }
    public decimal Value { get; set; }
    public Guid? DataBaseID { get; set; }
    
    [ForeignKey(nameof(DataBaseID))]
    public DataBaseEntity? DataBase { get; set; }
}

public enum ReferenceType
{
    [Display(Name = "Оперативная память")]
    Free,
    [Display(Name = "Накопительная память")]
    Df,
    [Display(Name = "Коэффициэнт кэширования")]
    CachingRatio,
    CachingIndexesRatio,
    ProcessTimeInSeconds
}