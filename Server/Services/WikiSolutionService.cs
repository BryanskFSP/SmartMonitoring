using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Interfaces;

namespace SmartMonitoring.Server.Services;

public class WikiSolutionService : IService<WikiSolutionEntity, WikiSolutionEditModel, Guid>
{
    private SMContext Context;
    private IMapper Mapper;

    public WikiSolutionService(SMContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public List<WikiSolutionEntity> GetAll()
    {
        return Context.WikiSolutions.AsNoTracking().ToList();
    }

    public string GetAllHash()
    {
        throw new NotImplementedException();
    }

    public List<WikiSolutionEntity> GetAllFull()
    {
        return Context.WikiSolutions.AsNoTracking()
            .Include(x => x.Wiki)
            .ToList();
    }

    public string GetAllFullHash()
    {
        throw new NotImplementedException();
    }

    public async Task<WikiSolutionEntity?> GetByID(Guid id)
    {
        return await Context.WikiSolutions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public Task<string> GetByIDHash(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<WikiSolutionEntity?> GetByIDFull(Guid id)
    {
        return await Context.WikiSolutions.AsNoTracking()
            .Include(x => x.Wiki)
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public Task<string> GetByIDFullHash(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<WikiSolutionEntity?> Update(Guid id, WikiSolutionEditModel editModel)
    {
        var entity = Mapper.Map<WikiSolutionEntity>(editModel);
        entity.ID = id;

        // TODO проверки

        Context.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<WikiSolutionEntity?> Create(WikiSolutionEditModel editModel)
    {
        var entity = Mapper.Map<WikiSolutionEntity>(editModel);
        // TODO проверки

        Context.Add(entity);

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await Context.WikiSolutions.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return true;
    }
}