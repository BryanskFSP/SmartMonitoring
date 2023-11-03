using System.ComponentModel.DataAnnotations;

namespace SmartMonitoring.Shared.Models;

/// <summary>
/// Перечисление типа логов.
/// </summary>
public enum LogType
{
    [Display(Name = "Информация")]
    Info,
    [Display(Name = "Подробности")]
    Verbose,
    [Display(Name = "Ошибка")]
    Error,
    [Display(Name = "Фатальная ошибка")]
    Fatal
}