using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReferenceValueController : ControllerBase
{
    private ReferenceValuesService Service;

    public ReferenceValueController(ReferenceValuesService service)
    {
        Service = service;
    }

    /// <summary>
    /// Get all Reference value models.
    /// </summary>
    /// <returns>List of Reference value models.</returns>
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<ReferenceValueModel>>>> GetAll()
    {
        var datas = await Service.GetAll();
        return datas;
    }

    /// <summary>
    /// Get Reference value model by Type.
    /// </summary>
    /// <param name="type">Reference type.</param>
    /// <returns>Reference value model.</returns>
    [HttpGet("{type}")]
    public async Task<ActionResult<ReferenceValueModel>> GetByType(ReferenceType type)
    {
        var data = await Service.GetValue(type);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    /// <summary>
    /// Set Reference value Model.
    /// </summary>
    /// <param name="model">Reference Model.</param>
    /// <returns>No content</returns>
    [HttpPost]
    public async Task<IActionResult> Set([FromBody] ReferenceValueModel model)
    {
        await Service.SettingValueElement(model);

        return Ok();
    }
}