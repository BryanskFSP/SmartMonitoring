using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Extensions;
using SmartMonitoring.Shared.Interfaces;

namespace SmartMonitoring.Server.Services;

public class WikiService : IService<WikiEntity, WikiEditModel, Guid>
{
    private SMContext Context;
    private IMapper Mapper;
    private WikiSolutionService WSService;

    public WikiService(SMContext context, IMapper mapper, WikiSolutionService wsService)
    {
        Context = context;
        Mapper = mapper;
        WSService = wsService;
    }

    public List<WikiEntity> GetAll()
    {
        return Context.Wikies.AsNoTracking().ToList();
    }

    public string GetAllHash()
    {
        return GetAll().ToHashSHA256();
    }

    public List<WikiEntity> GetAllFull()
    {
        return Context.Wikies.AsNoTracking()
            .Include(x => x.WikiSolutions)
            .ToList();
    }

    public string GetAllFullHash()
    {
        return GetAllFull().ToHashSHA256();
    }

    public async Task<WikiEntity?> GetByID(Guid id)
    {
        return await Context.Wikies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDHash(Guid id)
    {
        return (await GetByID(id)).ToHashSHA256();
    }

    public async Task<WikiEntity?> GetByIDFull(Guid id)
    {
        return await Context.Wikies.AsNoTracking()
            .Include(x => x.WikiSolutions)
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDFullHash(Guid id)
    {
        return (await GetByIDFull(id)).ToHashSHA256();
    }

    public async Task<WikiEntity?> Update(Guid id, WikiEditModel editModel)
    {
        var entity = Mapper.Map<WikiEntity>(editModel);
        entity.ID = id;

        // TODO проверки
        
        entity.WikiSolutions?.Clear();

        Context.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;

        await Context.SaveChangesAsync();
        var wikiSolutions = Context.WikiSolutions.Where(x => x.WikiID == id).ToList();
        foreach (var wikiSolution in wikiSolutions)
        {
            await WSService.Delete(wikiSolution.ID);
        }

        foreach (var call in editModel.WikiSolutions)
        {
            call.WikiID = entity.ID;
            var res = await WSService.Create(call);
            if (res != null)
            {
                entity.WikiSolutions.Add(res);
            }
        }
        return entity;
    }
    
    public async Task<WikiEntity?> Create(WikiEditModel editModel)
    {
        var entity = Mapper.Map<WikiEntity>(editModel);
        // TODO проверки
        entity.WikiSolutions?.Clear();

        Context.Add((object)entity);

        await Context.SaveChangesAsync();
        
        foreach (var call in editModel.WikiSolutions)
        {
            call.WikiID = entity.ID;
            var res = await WSService.Create(call);
            if (res != null)
            {
                entity.WikiSolutions.Add(res);
            }
        }
        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await Context.Wikies.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return true;
    }
}