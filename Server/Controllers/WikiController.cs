using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WikiController : ControllerBase
{
    private WikiService Service;
    private IMapper Mapper;

    public WikiController(WikiService service, IMapper mapper)
    {
        Service = service;
        Mapper = mapper;
    }

    /// <summary>
    /// Get all Wikis.
    /// </summary>
    /// <returns>List of Wiki models.</returns>
    [HttpGet]
    public async Task<ActionResult<List<WikiViewModel>>> GetAll()
    {
        var datas = Service.GetAll().Select(x => Mapper.Map<WikiViewModel>(x)).ToList();
        return datas;
    }

    /// <summary>
    /// Get all Full Wikis.
    /// </summary>
    /// <returns>List of Full Wiki models.</returns>
    [HttpGet("full")]
    public async Task<ActionResult<List<WikiViewModel>>> GetFull()
    {
        var datas = Service.GetAllFull().Select(x => Mapper.Map<WikiViewModel>(x)).ToList();
        return Ok(datas);
    }

    /// <summary>
    /// Get Wiki by ID.
    /// </summary>
    /// <param name="id">Wiki ID.</param>
    /// <returns>Wiki model.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<WikiViewModel>> GetByID(Guid id)
    {
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<WikiViewModel>(data));
    }
    
    [HttpGet("{id}/full")]
    public async Task<ActionResult<WikiViewModel>> GetByIDFull(Guid id)
    {
        var data = await Service.GetByIDFull(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<WikiViewModel>(data));
    }

    /// <summary>
    /// Create Wiki.
    /// </summary>
    /// <param name="editModel">Edit model of Wiki.</param>
    /// <returns>Wiki model.</returns>
    [HttpPost]
    public async Task<ActionResult<WikiViewModel>> Create([FromBody] WikiEditModel editModel)
    {
        var data = await Service.Create(editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<WikiViewModel>(data));
    }

    /// <summary>
    /// Update Wiki.
    /// </summary>
    /// <param name="id">Wiki ID.</param>
    /// <param name="editModel">Edit model of Wiki.</param>
    /// <returns>Wiki model</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<WikiViewModel>> Edit(Guid id, [FromBody] WikiEditModel editModel)
    {
        var data = await Service.Update(id, editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<WikiViewModel>(data));
    }

    /// <summary>
    /// Delete Wiki by ID.
    /// </summary>
    /// <param name="id">Wiki ID.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var data = await Service.Delete(id);
        return data ? NoContent() : NotFound("Сущность не найдена");
    }
}