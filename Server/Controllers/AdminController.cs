using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private AdminService Service;
    private IMapper Mapper;

    public AdminController(AdminService service, IMapper mapper)
    {
        Service = service;
        Mapper = mapper;
    }

    /// <summary>
    /// Get all Admins.
    /// </summary>
    /// <returns>List of Admin models.</returns>
    [HttpGet]
    public async Task<ActionResult<List<AdminViewModel>>> GetAll()
    {
        var datas = Service.GetAll().Select(x => Mapper.Map<AdminViewModel>(x)).ToList();
        return datas;
    }

    /// <summary>
    /// Get all Full Admins.
    /// </summary>
    /// <returns>List of Full Admin models.</returns>
    [HttpGet("full")]
    public async Task<ActionResult<List<AdminViewModel>>> GetFull()
    {
        var datas = Service.GetAllFull().Select(x => Mapper.Map<AdminViewModel>(x)).ToList();
        return Ok(datas);
    }

    /// <summary>
    /// Get Admin by ID.
    /// </summary>
    /// <param name="id">Admin ID.</param>
    /// <returns>Admin model.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AdminViewModel>> GetByID(Guid id)
    {
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<AdminViewModel>(data));
    }

    /// <summary>
    /// Create Admin.
    /// </summary>
    /// <param name="editModel">Edit model of Admin.</param>
    /// <returns>Admin model.</returns>
    [HttpPost]
    public async Task<ActionResult<AdminViewModel>> Create([FromBody] AdminEditModel editModel)
    {
        var data = await Service.Create(editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<AdminViewModel>(data));
    }

    // PUT: api/Admin/5
    [HttpPut("{id}")]
    public async Task<ActionResult<AdminViewModel>> Edit(Guid id, [FromBody] AdminEditModel editModel)
    {
        var data = await Service.Update(id, editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<AdminViewModel>(data));
    }

    // DELETE: api/Admin/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var data = await Service.Delete(id);
        return data ? NoContent() : NotFound("Сущность не найдена");
    }
}