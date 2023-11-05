using System.ComponentModel.DataAnnotations;

namespace SmartMonitoring.Shared.Models;

public class ReferenceValueModel
{
    public Guid ID { get; set; }
    public ReferenceType Type { get; set; }
    public decimal Value { get; set; }
    public Guid? DataBaseID { get; set; }
    

    public override int GetHashCode()
    {
        return (int)Type;
    }
}

public enum ReferenceType
{
    [Display(Name = "Оперативная память")]
    Free,
    [Display(Name = "Накопительная память")]
    Df,
    [Display(Name = "Коэффициэнт кэширования")]
    CachingRatio,
    [Display(Name = "Коэффициэнт кэширования индексов")]
    CachingIndexesRatio,
    [Display(Name = "Время выполнения запроса")]
    ProcessTimeInSeconds
}