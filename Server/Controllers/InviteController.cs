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

    /// <summary>
    /// Get all Invites.
    /// </summary>
    /// <returns>List of Invite.</returns>
    [HttpGet]
    public async Task<ActionResult<List<InviteViewModel>>> GetAll()
    {
        var datas = Service.GetAll().Select(x => Mapper.Map<InviteViewModel>(x)).ToList();
        return datas;
    }

    /// <summary>
    /// Get Hash by all Invites.
    /// </summary>
    /// <returns>Hash by all Invites.</returns>
    [HttpGet("hash")]
    public async Task<ActionResult<string>> GetAllHash()
    {
        return Service.GetAllHash();
    }

    /// <summary>
    /// Get all Full invites.
    /// </summary>
    /// <returns>List of Full invite.</returns>
    [HttpGet("full")]
    public async Task<ActionResult<List<InviteViewModel>>> GetFull()
    {
        var datas = Service.GetAllFull().Select(x => Mapper.Map<InviteViewModel>(x)).ToList();
        return Ok(datas);
    }
    
    /// <summary>
    /// Get Hash by all Full Invites.
    /// </summary>
    /// <returns>Hash by all Full Invites.</returns>
    [HttpGet("full/hash")]
    public async Task<ActionResult<string>> GetFullHash()
    {
        return Service.GetAllFullHash();
    }

    /// <summary>
    /// Get Invite by ID.
    /// </summary>
    /// <param name="id">Invite ID.</param>
    /// <returns>Invite model.</returns>
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
    
    /// <summary>
    /// Get Hash by Invite with ID.
    /// </summary>
    /// <param name="id">Invite ID.</param>
    /// <returns>Hash of Invite.</returns>
    [HttpGet("{id}/hash")]
    public async Task<ActionResult<string>> GetByIDHash(Guid id)
    {
        return await Service.GetByIDHash(id);
    }
    
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
    public async Task<ActionResult<InviteViewModel>> Create(InviteEditModel editModel)
    {
        var data = await Service.Create(editModel);
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