using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
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
    private PSQLService PsqlService;

    private readonly IHubContext<LogHub> LogHub;
    public List<TelegramUserEntity> TelegramUsers { get; set; } = new();

    public LogService(SMContext context, IMapper mapper, IHubContext<LogHub> logHub,
        TelegramUserService telegramUserService, IBot botApi, PSQLService psqlService)
    {
        Context = context;
        Mapper = mapper;
        LogHub = logHub;
        TelegramUserService = telegramUserService;
        BotApi = botApi;
        PsqlService = psqlService;

        TelegramUsers = TelegramUserService.GetAll();
    }

    /// <summary>
    /// Add Log.
    /// </summary>
    /// <param name="model">Log edit model</param>
    /// <returns>Log model.</returns>
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
                try
                {
                    await BotApi.SendMessageInUser(user.TelegramID, entity.Description, entity.ID);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error with send Bot data");
                }
            }
        }

        try
        {
            await LogHub.Clients.All.SendAsync("Add", res);
            await LogHub.Clients.All.SendAsync("AddID", res.ID.ToString());
            Log.Information("Send Data by SignalR");
        }
        catch (Exception e)
        {
            Log.Error(e, "SignalR Send error");
        }

        return res;
    }

    /// <summary>
    /// Get Log by ID.
    /// </summary>
    /// <param name="id">Log ID.</param>
    /// <returns>Log model.</returns>
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

    /// <summary>
    /// Get all Logs.
    /// </summary>
    /// <returns>List of Log models.</returns>
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

    /// <summary>
    /// Get all Logs by DataBase ID.
    /// </summary>
    /// <param name="dataBaseID">DataBase ID.</param>
    /// <returns>List of Log models.</returns>
    public async Task<List<LogViewModel>> GetAllByDataBaseID(Guid dataBaseID)
    {
        var entities = Context.Logs.AsNoTracking()
            .Where(x => x.DataBaseID == dataBaseID)
            .ToList();
        if (!entities.Any())
        {
            return new();
        }

        return entities.Select(x => Mapper.Map<LogViewModel>(x)).ToList();
    }


    /// <summary>
    /// Get all Logs.
    /// </summary>
    /// <returns>List of Logs.</returns>
    public async Task<List<LogViewModel>> GetAllFull()
    {
        var entities = Context.Logs.AsNoTracking()
            .Include(x => x.Organization)
            .Include(x => x.DataBase)
            .ToList();
        if (!entities.Any())
        {
            return new();
        }

        return entities.Select(x => Mapper.Map<LogViewModel>(x)).ToList();
    }


    /// <summary>
    /// Update Log entity.
    /// </summary>
    /// <param name="id">Log ID</param>
    /// <returns>Log model.</returns>
    private async Task<LogViewModel?> Fix(Guid id)
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

    /// <summary>
    /// Fix error by Log ID.
    /// </summary>
    /// <param name="id">Log ID.</param>
    /// <returns>Service response.</returns>
    public async Task<ServiceResponse<string>> FixError(Guid id)
    {
        var res = new ServiceResponse<string>();

        var entity = await GetByID(id);
        if (entity == null)
        {
            res.Name = "Сущность не найдена";
            return res;
        }

        if (entity.Action == ActionType.KillInfinityLoop)
        {
            await PsqlService.KillProcess(entity.DataBaseID.Value, entity.EntityID);
            res.Name = "Процесс успешно убит";
            res.Status = true;

            entity = await Fix(id);
        }
        else if (entity.Action == ActionType.NoSpace)
        {
            await PsqlService.ClearSpace(entity.DataBaseID.Value);
            res.Name = "Очистка началась!";
            res.Status = true;
            entity = await Fix(id);
        }

        return res;
    }
}