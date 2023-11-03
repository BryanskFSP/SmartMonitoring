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
    private IMapper Mapper;

    public DataBaseController(DataBaseService service, IMapper mapper)
    {
        Service = service;
        Mapper = mapper;
    }

    // GET: api/DataBase
    [HttpGet]
    public async Task<ActionResult<List<DataBaseViewModel>>> GetAll()
    {
        var datas = Service.GetAll().Select(x => Mapper.Map<DataBaseViewModel>(x)).ToList();
        return datas;
    }

    [HttpGet("full")]
    public async Task<ActionResult<List<DataBaseViewModel>>> GetFull()
    {
        var datas = Service.GetAllFull().Select(x => Mapper.Map<DataBaseViewModel>(x)).ToList();
        return Ok(datas);
    }

    // GET: api/DataBase?id=5
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

    // POST: api/DataBase
    [HttpPost]
    public async Task<ActionResult<DataBaseViewModel>> Create([FromBody] DataBaseEditModel editModel)
    {
        var data = await Service.Create(editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<DataBaseViewModel>(data));
    }

    // PUT: api/DataBase/5
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

    // DELETE: api/DataBase/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var data = await Service.Delete(id);
        return data ? NoContent() : NotFound("Сущность не найдена");
    }
}