using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PSQLController: ControllerBase
{
    private PSQLService Service;

    public PSQLController(PSQLService service)
    {
        Service = service;
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
    /// Get Caching ratio from Main DB.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpGet("tables/cachingratio")]
    public async Task<ActionResult<ServiceResponse<decimal>>> GetCachingRatio(Guid dbID)
    {
        var datas = await Service.GetCachingRatio(dbID);

        return datas;
    }
    
    /// <summary>
    /// Get Caching indexes ratio from Main DB.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpGet("tables/cachingindexesratio")]
    public async Task<ActionResult<ServiceResponse<decimal>>> GetCachingIndexesRatio(Guid dbID)
    {
        var datas = await Service.GetCachingIndexesRatio(dbID);

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
    
    /// <summary>
    /// Clear Space in DataBase.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpPost("space/clear")]
    public async Task<IActionResult> ClearSpace(Guid dbID)
    {
        await Service.ClearSpace(dbID);
        return Ok();
    }
    
    /// <summary>
    /// Clear Space by vacuum in DataBase.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpPost("space/clear/vacuum")]
    public async Task<IActionResult> ClearSpaceByVacuum(Guid dbID)
    {
        await Service.ClearSpaceByVacuum(dbID);
        return Ok();
    }

    /// <summary>
    /// Get blocked Processes in DataBase.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpPost("processes/locked")]
    public async Task<ActionResult<ServiceResponse<List<PSQLLockModel>>>> GetLockProcesses(Guid dbID)
    {
        var res = await Service.GetLockProcesses(dbID);
        return res;
    }
    
    
    /// <summary>
    /// Get indexes stats in DataBase.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpPost("stats/indexes")]
    public async Task<ActionResult<ServiceResponse<List<IndexModel>>>> GetIndexesStats(Guid dbID)
    {
        var res = await Service.GetIndexesStats(dbID);
        return res;
    }
    
    /// <summary>
    /// Get outdated indexes stats in DataBase.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpPost("stats/indexes/outdated")]
    public async Task<ActionResult<ServiceResponse<List<OutdatedIndexModel>>>> GetOutdatedIndexesStats(Guid dbID)
    {
        var res = await Service.GetOutdatedIndexesStats(dbID);
        return res;
    }
    
    /// <summary>
    /// Get wasted bytes in DataBase.
    /// </summary>
    /// <param name="dbID">ID Database.</param>
    /// <returns></returns>
    [HttpPost("wasted")]
    public async Task<ActionResult<ServiceResponse<decimal>>> GetWastedBytes(Guid dbID)
    {
        var res = await Service.GetWastedBytes(dbID);
        return res;
    }


}

public class DataBaseStatsModel
{
    // TODO все данные статистики
}