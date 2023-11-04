using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataBaseController : ControllerBase
{
    private DataBaseService Service;
    private PSQLService PsqlService;
    private IMapper Mapper;

    public DataBaseController(DataBaseService service, IMapper mapper)
    {
        Service = service;
        Mapper = mapper;
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
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var data = await Service.Delete(id);
        return data ? NoContent() : NotFound("Сущность не найдена");
    }
}