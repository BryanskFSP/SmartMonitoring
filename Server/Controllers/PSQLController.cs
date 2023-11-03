using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PSQLController: ControllerBase
{
    private PSQLService Service;
    private IMapper Mapper;

    public PSQLController(PSQLService service, IMapper mapper)
    {
        Service = service;
        Mapper = mapper;
    }

    
    /// <summary>
    /// Get States from Main DB.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<PGStatActivityModel>>> GetStates()
    {
        var datas = await Service.GetModelsActive();

        return datas;
    }

    
    /// <summary>
    /// Kill State in Main DB.
    /// </summary>
    /// <param name="pid">PID process.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> KillState(long pid)
    {
       await Service.KillProcess(pid);
       return Ok();
    }
}