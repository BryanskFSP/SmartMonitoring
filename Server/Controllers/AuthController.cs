using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.Models;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private AuthService AuthService;

    public AuthController(AuthService authService)
    {
        AuthService = authService;
    }

    /// <summary>
    /// Auth User (admin) by auth data.
    /// </summary>
    /// <param name="model">Auth data.</param>
    /// <returns>Auth helper model.</returns>
    [HttpPost("user")]
    public async Task<ActionResult<ServiceResponse<AuthHelperModel<AdminViewModel>>>> AuthUser(AuthModel model)
    {
        return await AuthService.Authenticate(model);
    }
}