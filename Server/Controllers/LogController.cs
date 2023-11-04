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
    private PSQLService PsqlService;
    private IMapper Mapper;
    private LogService Service;

    public LogController(IMapper mapper, LogService service, PSQLService psqlService)
    {
        Mapper = mapper;
        Service = service;
        PsqlService = psqlService;
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
        var res = new ServiceResponse<string>();

        var entity = await Service.GetByID(id);
        if (entity == null)
        {
            res.Name = "Сущность не найдена";
            return NotFound(res);
        }

        if (entity.Action == ActionType.KillInfinityLoop)
        {
            await PsqlService.KillProcess(entity.DataBaseID.Value, entity.EntityID);
            res.Name = "Процесс успешно убит";
            res.Status = true;
            
            entity = await Service.Fix(id);
        }
        else if (entity.Action == ActionType.NoSpace)
        {
            await PsqlService.ClearSpace(entity.DataBaseID.Value);
            res.Name = "Очистка началась!";
            res.Status = true;
            entity = await Service.Fix(id);
        }

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
}