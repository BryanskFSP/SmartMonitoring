using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Extensions;
using SmartMonitoring.Shared.Interfaces;

namespace SmartMonitoring.Server.Services;

public class DataBaseService : IService<DataBaseEntity, DataBaseEditModel, Guid>
{
    private SMContext Context;
    private IMapper Mapper;

    public DataBaseService(SMContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public List<DataBaseEntity> GetAll()
    {
        return Context.DataBases.AsNoTracking().ToList();
    }

    public string GetAllHash()
    {
        return GetAll().ToHashSHA256();
    }

    public List<DataBaseEntity> GetAllFull()
    {
        return Context.DataBases.AsNoTracking()
            .ToList();
    }

    public string GetAllFullHash()
    {
        return GetAllFull().ToHashSHA256();
    }

    public async Task<DataBaseEntity?> GetByID(Guid id)
    {
        return await Context.DataBases.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDHash(Guid id)
    {
        return (await GetByID(id)).ToHashSHA256();
    }

    public async Task<DataBaseEntity?> GetByIDFull(Guid id)
    {
        return await Context.DataBases.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDFullHash(Guid id)
    {
        return (await GetByIDFull(id)).ToHashSHA256();
    }

    public async Task<DataBaseEntity?> Update(Guid id, DataBaseEditModel editModel)
    {
        var entity = Mapper.Map<DataBaseEntity>(editModel);
        entity.ID = id;

        // TODO проверки

        Context.Entry(entity).Property(x => x.OrganizationID).IsModified = false;

        Context.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<DataBaseEntity?> Create(DataBaseEditModel editModel)
    {
        var entity = Mapper.Map<DataBaseEntity>(editModel);
        // TODO проверки

        Context.Add((object)entity);

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await Context.DataBases.FindAsync(id);
        if (entity == null)
        {
            return false;
        }
        
        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return true;
    }
}