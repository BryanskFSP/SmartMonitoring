using System.ComponentModel.DataAnnotations;

namespace SmartMonitoring.Shared.EditModels;

public class AdminEditModel
{
    public string Name { get; set; }
    public string Login { get; set; }
    [MinLength(6), MaxLength(32)]
    [Display(Name = "Пароль")]
    public string? Password { get; set; } = null!;

    [Display(Name = "Повторите пароль")]
    [Compare(nameof(Password))] 
    public string? RepeatPassword { get; set; } = null!;
}