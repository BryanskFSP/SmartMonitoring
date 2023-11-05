using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TelegramUserController : ControllerBase
{
    private TelegramUserService Service;
    private InviteService InviteService;
    private IMapper Mapper;

    public TelegramUserController(TelegramUserService service, IMapper mapper, InviteService inviteService)
    {
        Service = service;
        Mapper = mapper;
        InviteService = inviteService;
    }
    
    /// <summary>
    /// Get Telegram Users.
    /// </summary>
    /// <returns>List of Telegram Users.</returns>
    [HttpGet]
    public async Task<ActionResult<List<TelegramUserViewModel>>> GetAll()
    {
        var datas = Service.GetAll().Select(x => Mapper.Map<TelegramUserViewModel>(x)).ToList();
        return datas;
    }

    /// <summary>
    /// Get Hash of Telegram Users.
    /// </summary>
    /// <returns>Hash of Telegram Users.</returns>
    [HttpGet("hash")]
    public async Task<ActionResult<string>> GetAllHash()
    {
        return Service.GetAllHash();
    }

    /// <summary>
    /// Get Full Telegram Users.
    /// </summary>
    /// <returns>List of Telegram Users.</returns>
    [HttpGet("full")]
    public async Task<ActionResult<List<TelegramUserViewModel>>> GetFull()
    {
        var datas = Service.GetAllFull().Select(x => Mapper.Map<TelegramUserViewModel>(x)).ToList();
        return Ok(datas);
    }
    
    /// <summary>
    /// Get Full Hash of Telegram Users.
    /// </summary>
    /// <returns>Full Hash of Telegram Users.</returns>
    [HttpGet("full/hash")]
    public async Task<ActionResult<string>> GetFullHash()
    {
        return Service.GetAllFullHash();
    }

    /// <summary>
    /// Get Telegram User by ID.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <returns>Telegram User.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TelegramUserViewModel>> GetByID(int id)
    {
        var data = await Service.GetByID(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<TelegramUserViewModel>(data));
    }
    
    /// <summary>
    /// Get Hash Telegram User by ID.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <returns>Hash of Telegram User.</returns>
    [HttpGet("{id}/hash")]
    public async Task<ActionResult<string>> GetByIDHash(int id)
    {
        return await Service.GetByIDHash(id);
    }
    
    /// <summary>
    /// Get Full Telegram User by ID.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <returns>Full Telegram User.</returns>
    [HttpGet("{id}/full")]
    public async Task<ActionResult<TelegramUserViewModel>> GetByIDFull(int id)
    {
        var data = await Service.GetByIDFull(id);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<TelegramUserViewModel>(data));
    }

    /// <summary>
    /// Get Full Hash Telegram User by ID.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <returns>Full Hash of Telegram User.</returns>
    [HttpGet("{id}/full/hash")]
    public async Task<ActionResult<string>> GetByIDFullHash(int id)
    {
        return await Service.GetByIDFullHash(id);
    }
    
    /// <summary>
    /// Create Telegram User.
    /// </summary>
    /// <param name="inviteCode">Invite code.</param>
    /// <param name="editModel">Edit model of Telegram User.</param>
    /// <returns>View model of Telegram User.</returns>
    [HttpPost]
    public async Task<ActionResult<TelegramUserViewModel>> Create(string inviteCode, [FromBody] TelegramUserEditModel editModel)
    {
        var invite = await InviteService.GetByCode(inviteCode);
        if (invite == null)
        {
            return Forbid();
        }

        var inveditModel = Mapper.Map<InviteEditModel>(invite);
        inveditModel.UsedCount += 1;
        await InviteService.Update(invite.ID, inveditModel);
        
        var data = await Service.Create(editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<TelegramUserViewModel>(data));
    }

    /// <summary>
    /// Update Telegram User by ID.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <param name="editModel">Edit model of Telegram User.</param>
    /// <returns>View model of Telegram User.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<TelegramUserViewModel>> Edit(int id, [FromBody] TelegramUserEditModel editModel)
    {
        var data = await Service.Update(id, editModel);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<TelegramUserViewModel>(data));
    }

    /// <summary>
    /// Delete Telegram User by ID.
    /// </summary>
    /// <param name="id">Telegram User ID.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var data = await Service.Delete(id);
        return data ? NoContent() : NotFound("Сущность не найдена");
    }
}