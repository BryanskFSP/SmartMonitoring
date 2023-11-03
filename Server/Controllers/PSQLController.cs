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
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<PGStatActivityModel>>> GetStates(Guid dbID)
    {
        var datas = await Service.GetModelsActive(dbID);

        return datas;
    }
    
    /// <summary>
    /// Get Memory States from Main DB.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <param name="memoryType">Memory type.</param>
    /// <returns></returns>
    [HttpGet("tables/memory")]
    public async Task<ActionResult<ServiceResponse<MemoryInfoModel>>> GetMemory(Guid dbID, MemoryType memoryType)
    {
        var datas = await Service.GetMemoryInfo(dbID, memoryType);

        return datas;
    }
    
    /// <summary>
    /// Get Table States from Main DB.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpGet("tables/top")]
    public async Task<ActionResult<ServiceResponse<List<TableStatsModel>>>> GetTablesTop(Guid dbID)
    {
        var datas = await Service.GetTopOperationsInTables(dbID);

        return datas;
    }
    
    /// <summary>
    /// Kill State in Main DB.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <param name="pid">PID process.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> KillState(Guid dbID,long pid)
    {
       await Service.KillProcess(dbID, pid.ToString());
       return Ok();
    }
    
    /// <summary>
    /// Create infinity loop in Database.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpPost("errors/create/infinityloop/{dbID}")]
    public async Task<IActionResult> CreateInfinityLoop(Guid dbID)
    {
        await Task.Run(async()=> await Service.CreateInfinityLoop(dbID));

        return Ok();
    }
}