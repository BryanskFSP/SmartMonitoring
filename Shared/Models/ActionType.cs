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
    DataBase,
    RunDataBase,
    KillDataBase,
    RestartDataBase,
    KillInfinityLoop
}