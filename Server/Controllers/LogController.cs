using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private SMContext Context;
    private IMapper Mapper;
    private LogService Service;

    public LogController(SMContext context, IMapper mapper, LogService service)
    {
        Context = context;
        Mapper = mapper;
        Service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<LogViewModel>>> GetAll()
    {
        var logs = await Service.GetAll();

        return logs;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LogViewModel>> GetByID(Guid id)
    {
        var log = await Service.GetByID(id);

        if (log == null)
        {
            return NotFound();
        }

        return Mapper.Map<LogViewModel>(log);
    }
}