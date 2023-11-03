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

    [HttpPost("user")]
    public async Task<ActionResult<ServiceResponse<AuthHelperModel<AdminViewModel>>>> AuthUser(AuthModel model)
    {
        return await AuthService.Authenticate(model);
    }
}