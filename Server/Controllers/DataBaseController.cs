using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Models;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataBaseController : ControllerBase
{
    private DataBaseService Service;
    private PSQLService PsqlService;
    private PSQLCheckerService PsqlCheckerService;
    private LogService LogService;
    private IMapper Mapper;

    public DataBaseController(DataBaseService service, IMapper mapper, PSQLService psqlService, PSQLCheckerService psqlCheckerService, LogService logService)
    {
        Service = service;
        Mapper = mapper;
        PsqlService = psqlService;
        PsqlCheckerService = psqlCheckerService;
        LogService = logService;
    }

    /// <summary>
    /// Get all DataBases.
    /// </summary>
    /// <returns>List of Database.</returns>
    [HttpGet]
    public async Task<ActionResult<List<DataBaseViewModel>>> GetAll()
    {
        var datas = Service.GetAll().Select(x => Mapper.Map<DataBaseViewModel>(x)).ToList();
        return datas;
    }

    /// <summary>
    /// Get all Full DataBases.
    /// </summary>
    /// <returns></returns>
    [HttpGet("full")]
    public async Task<ActionResult<List<DataBaseViewModel>>> GetFull()
    {
        var datas = Service.GetAllFull().Select(x => Mapper.Map<DataBaseViewModel>(x)).ToList();
        return Ok(datas);
    }

    /// <summary>
    /// Get DataBase by ID.
    /// </summary>
    /// <param name="id">DataBase ID.</param>
    /// <returns>DataBase model.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<DataBaseViewModel>> GetByID(Guid id)
    {
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<DataBaseViewModel>(data));
    }

    /// <summary>
    /// Create DataBase.
    /// </summary>
    /// <param name="editModel">Edit model of DataBase.</param>
    /// <returns>DataBase model.</returns>
    [HttpPost]
    public async Task<ActionResult<DataBaseViewModel>> Create([FromBody] DataBaseEditModel editModel)
    {
        var data = await Service.Create(editModel);
        await PsqlService.CreateFunctions(data.ID);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<DataBaseViewModel>(data));
    }

    /// <summary>
    /// Update DataBase by ID.
    /// </summary>
    /// <param name="id">DataBase ID.</param>
    /// <param name="editModel">Edit model of DataBase.</param>
    /// <returns>DataBase model.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<DataBaseViewModel>> Edit(Guid id, [FromBody] DataBaseEditModel editModel)
    {
        var data = await Service.Update(id, editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<DataBaseViewModel>(data));
    }

    /// <summary>
    /// Delete DataBase by ID.
    /// </summary>
    /// <param name="id">DataBase ID.</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var data = await Service.Delete(id);
        return data ? NoContent() : NotFound("Сущность не найдена");
    }

    /// <summary>
    /// Starting a check Full DataBase.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content</returns>
    [HttpPost("{id}/check/full")]
    public async Task<ActionResult<ServiceResponse<string>>> CheckFull(Guid id)
    {
        var res =new ServiceResponse<string>();
        var sb = new StringBuilder();
        
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        await PsqlCheckerService.CheckMemory(MemoryType.HDD, data);
        await PsqlCheckerService.CheckState(data);
        await PsqlCheckerService.CheckingCachingRatio(data);
        await PsqlCheckerService.CheckingCachingIndexesRatio(data);

        return res;
    }
    
    /// <summary>
    /// Starting a check memory in DataBase.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content</returns>
    [HttpPost("{id}/check/memory")]
    public async Task<ActionResult<ServiceResponse<string>>> CheckMemory(Guid id, MemoryType memoryType)
    {
        var res =new ServiceResponse<string>();
        var sb = new StringBuilder();
        
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        await PsqlCheckerService.CheckMemory(memoryType, data);

        return res;
    }
    
    /// <summary>
    /// Starting a check states in DataBase.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content</returns>
    [HttpPost("{id}/check/states")]
    public async Task<ActionResult<ServiceResponse<string>>> CheckStates(Guid id)
    {
        var res =new ServiceResponse<string>();
        var sb = new StringBuilder();
        
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        await PsqlCheckerService.CheckState(data);

        return res;
    }
    
    /// <summary>
    /// Starting a check Caching Ratio in DataBase.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content</returns>
    [HttpPost("{id}/check/cachingratio")]
    public async Task<ActionResult<ServiceResponse<string>>> CheckCachingRatio(Guid id)
    {
        var res =new ServiceResponse<string>();
        var sb = new StringBuilder();
        
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        await PsqlCheckerService.CheckingCachingRatio(data);

        return res;
    }
    
    /// <summary>
    /// Starting a check Caching Indexes Ratio in DataBase.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content</returns>
    [HttpPost("{id}/check/cachingindexesratio")]
    public async Task<ActionResult<ServiceResponse<string>>> CheckCachingIndexesRatio(Guid id)
    {
        var res =new ServiceResponse<string>();
        var sb = new StringBuilder();
        
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        await PsqlCheckerService.CheckingCachingIndexesRatio(data);

        return res;
    }

    [HttpPost("{id}/fix/full")]
    public async Task<ActionResult<ServiceResponse<string>>> FullFix(Guid id)
    {
        var res =new ServiceResponse<string>();
        var sb = new StringBuilder();
                  var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        var logs = await LogService.GetAllByDataBaseID(id);
        
        foreach (var log in logs)
        {
            var result = await LogService.FixError(log.ID);
            sb.AppendLine(result.Data);
        }

        res.Status = true;
        return res;
    }
}