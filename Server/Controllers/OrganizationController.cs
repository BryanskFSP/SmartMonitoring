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
public class OrganizationController : ControllerBase
{
    private OrganizationService Service;
    private IMapper Mapper;

    public OrganizationController(OrganizationService service, IMapper mapper)
    {
        Service = service;
        Mapper = mapper;
    }

    // GET: api/Organization
    [HttpGet]
    public async Task<ActionResult<List<OrganizationViewModel>>> GetAll()
    {
        var datas = Service.GetAll().Select(x => Mapper.Map<OrganizationViewModel>(x)).ToList();
        return datas;
    }

    [HttpGet("full")]
    public async Task<ActionResult<List<OrganizationViewModel>>> GetFull()
    {
        var datas = Service.GetAllFull().Select(x => Mapper.Map<OrganizationViewModel>(x)).ToList();
        return Ok(datas);
    }

    // GET: api/Organization?id=5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationViewModel>> GetByID(Guid id)
    {
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<OrganizationViewModel>(data));
    }

    // POST: api/Organization
    [HttpPost]
    public async Task<ActionResult<OrganizationViewModel>> Create([FromBody] OrganizationEditModel editModel)
    {
        var data = await Service.Create(editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<OrganizationViewModel>(data));
    }

    // PUT: api/Organization/5
    [HttpPut("{id}")]
    public async Task<ActionResult<OrganizationViewModel>> Edit(Guid id, [FromBody] OrganizationEditModel editModel)
    {
        var data = await Service.Update(id, editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<OrganizationViewModel>(data));
    }

    // DELETE: api/Organization/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var data = await Service.Delete(id);
        return data ? NoContent() : NotFound("Сущность не найдена");
    }
}