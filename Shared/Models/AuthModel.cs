using System.ComponentModel.DataAnnotations;

namespace SmartMonitoring.Shared.Models;

public class AuthModel
{
    [Required]
    public string Login { get; set; }
    [Required]
    public string Password { get; set; }
}