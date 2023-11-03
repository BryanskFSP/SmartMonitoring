using System.ComponentModel.DataAnnotations;

namespace SmartMonitoring.Shared.Models;

/// <summary>
/// Перечисление типа действия.
/// </summary>
public enum ActionType
{
    [Display(Name = "Изменение")]
    Edit,
    [Display(Name = "Удаление")]
    Delete,
    [Display(Name = "База данных")]
    DataBase,
    [Display(Name = "Запуск Базы данных")]
    RunDataBase,
    [Display(Name = "Kill Базы данных")]
    KillDataBase,
    [Display(Name = "Restart Базы данных")]
    RestartDataBase,
    [Display(Name = "Долгий запрос")]
    KillInfinityLoop,
    [Display(Name = "Нет места")]
    NoSpace,
    [Display(Name = "Коэф. кэша")]
    CachingRatio,
    [Display(Name = "Коэф. кэша индексов")]
    CachingIndexRatio,
}