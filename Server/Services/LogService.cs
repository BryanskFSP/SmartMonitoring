using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server.Services;

/// <summary>
/// Сервис по работе с логами.
/// </summary>
public class LogService
{
    private SMContext Context;
    private IMapper Mapper;

    public LogService(SMContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public async Task<LogViewModel> Add(LogEditModel model)
    {
        var entity = Mapper.Map<LogEntity>(model);
        entity.CreatedAt = DateTime.Now;
        
        Context.Logs.Add(entity);
        await Context.SaveChangesAsync();

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
}