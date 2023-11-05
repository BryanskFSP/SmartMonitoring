using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.Models;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private IMapper Mapper;
    private WikiSolutionService WikiSolutionService;
    private LogService Service;

    public LogController(IMapper mapper, LogService service, WikiSolutionService wikiSolutionService)
    {
        Mapper = mapper;
        Service = service;
        WikiSolutionService = wikiSolutionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LogViewModel>>> GetAll()
    {
        var logs = await Service.GetAll();

        return logs;
    }
    
    /// <summary>
    /// Get Full Telegram Users.
    /// </summary>
    /// <returns>List of Telegram Users.</returns>
    [HttpGet("full")]
    public async Task<ActionResult<List<TelegramUserViewModel>>> GetFull()
    {
        var datas = await Service.GetAllFull();
        return Ok(datas);
    }

    [HttpGet("db/{dbid}")]
    public async Task<ActionResult<List<LogViewModel>>> GetByDBID(Guid dbid)
    {
        var logs = await Service.GetAllFull();
        logs = logs.Where(x => x.DataBaseID == dbid).ToList();
        
        return logs;
    }

    /// <summary>
    /// Fix error in Log.
    /// </summary>
    /// <param name="id">Log ID.</param>
    /// <returns>Service response of try fix error.</returns>
    [HttpPost("{id}")]
    public async Task<ActionResult<ServiceResponse<string>>> FixError(Guid id)
    {
        var res = await Service.FixError(id);

        return res;
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

    [HttpPost("{logID}/wikisolution/{wikiSolutionID}")]
    public async Task<ActionResult<ServiceResponse<string>>> UseWikiSolution(Guid logID, Guid wikiSolutionID)
    {
        var log = await Service.GetByID(logID);

        if (log == null)
        {
            return NotFound();
        }

        var wikiSolution = await WikiSolutionService.GetByID(wikiSolutionID);
        if (wikiSolution == null)
        {
            return NotFound();
        }

        return await Service.RunSqlScript(log.DataBaseID.GetValueOrDefault(), wikiSolution.SqlScript);
    }
}