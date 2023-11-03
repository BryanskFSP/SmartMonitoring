using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Extensions;
using SmartMonitoring.Shared.Interfaces;

namespace SmartMonitoring.Server.Services;

public class OrganizationService : IService<OrganizationEntity, OrganizationEditModel, Guid>
{
    private SMContext Context;
    private IMapper Mapper;

    public OrganizationService(SMContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public List<OrganizationEntity> GetAll()
    {
        return Context.Organizations.AsNoTracking().ToList();
    }

    public string GetAllHash()
    {
        return GetAll().ToHashSHA256();
    }

    public List<OrganizationEntity> GetAllFull()
    {
        return Context.Organizations.AsNoTracking()
            .ToList();
    }

    public string GetAllFullHash()
    {
        return GetAllFull().ToHashSHA256();
    }

    public async Task<OrganizationEntity?> GetByID(Guid id)
    {
        return await Context.Organizations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDHash(Guid id)
    {
        return (await GetByID(id)).ToHashSHA256();
    }

    public async Task<OrganizationEntity?> GetByIDFull(Guid id)
    {
        return await Context.Organizations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDFullHash(Guid id)
    {
        return (await GetByIDFull(id)).ToHashSHA256();
    }

    public async Task<OrganizationEntity?> Update(Guid id, OrganizationEditModel editModel)
    {
        var entity = Mapper.Map<OrganizationEntity>(editModel);
        entity.ID = id;

        // TODO проверки

        Context.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<OrganizationEntity?> Create(OrganizationEditModel editModel)
    {
        var entity = Mapper.Map<OrganizationEntity>(editModel);
        // TODO проверки

        Context.Add((object)entity);

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await Context.Organizations.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return true;
    }
}