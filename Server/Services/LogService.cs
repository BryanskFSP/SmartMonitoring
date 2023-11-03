using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Server.Hubs;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Interfaces.Refit;
using SmartMonitoring.Shared.Models;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Services;

/// <summary>
/// Сервис по работе с логами.
/// </summary>
public class LogService
{
    private SMContext Context;
    private IMapper Mapper;
    private TelegramUserService TelegramUserService;
    private IBot BotApi;
    
    private readonly IHubContext<LogHub> LogHub;
    public List<TelegramUserEntity> TelegramUsers { get; set; } = new();
    
    public LogService(SMContext context, IMapper mapper, IHubContext<LogHub> logHub, TelegramUserService telegramUserService, IBot botApi)
    {
        Context = context;
        Mapper = mapper;
        LogHub = logHub;
        TelegramUserService = telegramUserService;
        BotApi = botApi;

        TelegramUsers = TelegramUserService.GetAll();
    }

    public async Task<LogViewModel> Add(LogEditModel model)
    {
        var entity = Mapper.Map<LogEntity>(model);
        entity.CreatedAt = DateTime.Now;
        
        Context.Logs.Add(entity);
        await Context.SaveChangesAsync();
        var res = Mapper.Map<LogViewModel>(entity);

        if (entity.LogType >= LogType.Error)
        {
            var users = TelegramUsers.Where(x => x.OrganizationID == res.OrganizationID).ToList();
            foreach (var user in users)
            {
                await BotApi.SendMessageInUser(user.TelegramID, entity.Description, entity.ID);
            }
        }
        
        await LogHub.Clients.All.SendAsync("Add", res);
        return Mapper.Map<LogViewModel>(entity);
    }

    public async Task<LogViewModel?> GetByID(Guid id)
    {
        var entity = Context.Logs.AsNoTracking()
            .FirstOrDefault(x => x.ID == id);
        if (entity == null)
        {
            return null;
        }

        return Mapper.Map<LogViewModel>(entity);
    }

    
    public async Task<List<LogViewModel>> GetAll()
    {
        var entities = Context.Logs.AsNoTracking()
            .ToList();
        if (!entities.Any())
        {
            return new();
        }

        return entities.Select(x => Mapper.Map<LogViewModel>(x)).ToList();
    }
    
    
    public async Task<List<LogViewModel>> GetAllFull()
    {
        var entities = Context.Logs.AsNoTracking()
            .Include(x=>x.Organization)
            .Include(x=>x.DataBase)
            .ToList();
        if (!entities.Any())
        {
            return new();
        }

        return entities.Select(x => Mapper.Map<LogViewModel>(x)).ToList();
    }


    public async Task<LogViewModel?> Fix(Guid id)
    {
        var entity = Context.Logs
            .FirstOrDefault(x => x.ID == id);
        if (entity == null)
        {
            return null;
        }
        
        entity.UpdatedAt = DateTime.Now;
        entity.FixStatus = true;
        await Context.SaveChangesAsync();
        var res = Mapper.Map<LogViewModel>(entity);
        
        
        await LogHub.Clients.All.SendAsync("Update", res);

        return res;
    }
}