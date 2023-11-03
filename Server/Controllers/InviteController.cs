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
public class InviteController : ControllerBase
{
    private InviteService Service;
    private IMapper Mapper;

    public InviteController(InviteService service, IMapper mapper)
    {
        Service = service;
        Mapper = mapper;
    }

    // GET: api/Invite
    [HttpGet]
    public async Task<ActionResult<List<InviteViewModel>>> GetAll()
    {
        var datas = Service.GetAll().Select(x => Mapper.Map<InviteViewModel>(x)).ToList();
        return datas;
    }

    [HttpGet("hash")]
    public async Task<ActionResult<string>> GetAllHash()
    {
        return Service.GetAllHash();
    }

    [HttpGet("full")]
    public async Task<ActionResult<List<InviteViewModel>>> GetFull()
    {
        var datas = Service.GetAllFull().Select(x => Mapper.Map<InviteViewModel>(x)).ToList();
        return Ok(datas);
    }
    
    [HttpGet("full/hash")]
    public async Task<ActionResult<string>> GetFullHash()
    {
        return Service.GetAllFullHash();
    }

    // GET: api/Invite?id=5
    [HttpGet("{id}")]
    public async Task<ActionResult<InviteViewModel>> GetByID(Guid id)
    {
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<InviteViewModel>(data));
    }
    
    [HttpGet("{id}/hash")]
    public async Task<ActionResult<string>> GetByIDHash(Guid id)
    {
        return await Service.GetByIDHash(id);
    }
    
    // GET: api/Invite?id=5
    [HttpGet("{id}/full")]
    public async Task<ActionResult<InviteViewModel>> GetByIDFull(Guid id)
    {
        var data = await Service.GetByIDFull(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<InviteViewModel>(data));
    }

    [HttpGet("{id}/full/hash")]
    public async Task<ActionResult<string>> GetByIDFullHash(Guid id)
    {
        return await Service.GetByIDFullHash(id);
    }
    
    // POST: api/Invite
    [HttpPost]
    public async Task<ActionResult<InviteViewModel>> Create()
    {
        var data = await Service.Create();
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<InviteViewModel>(data));
    }

    // DELETE: api/Invite/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var data = await Service.Delete(id);
        return data ? NoContent() : NotFound("Сущность не найдена");
    }
}