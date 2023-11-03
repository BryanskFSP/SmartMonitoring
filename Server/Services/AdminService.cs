using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Extensions;
using SmartMonitoring.Shared.Interfaces;

namespace SmartMonitoring.Server.Services;

public class AdminService : IService<AdminEntity, AdminEditModel, Guid>
{
    private SMContext Context;
    private IMapper Mapper;

    public AdminService(SMContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public List<AdminEntity> GetAll()
    {
        return Context.Admins.AsNoTracking().ToList();
    }

    public string GetAllHash()
    {
        return GetAll().ToHashSHA256();
    }

    public List<AdminEntity> GetAllFull()
    {
        return Context.Admins.AsNoTracking()
            .ToList();
    }

    public string GetAllFullHash()
    {
        return GetAllFull().ToHashSHA256();
    }

    public async Task<AdminEntity?> GetByID(Guid id)
    {
        return await Context.Admins.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDHash(Guid id)
    {
        return (await GetByID(id)).ToHashSHA256();
    }

    public async Task<AdminEntity?> GetByIDFull(Guid id)
    {
        return await Context.Admins.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDFullHash(Guid id)
    {
        return (await GetByIDFull(id)).ToHashSHA256();
    }

    public async Task<AdminEntity?> Update(Guid id, AdminEditModel editModel)
    {
        var entity = Mapper.Map<AdminEntity>(editModel);
        entity.ID = id;

        // TODO проверки

        if (!string.IsNullOrWhiteSpace(editModel.Password))
        {
            entity.PasswordHash = editModel.Password.ToHashSHA256();
        }
        else
        {
            Context.Entry(entity).Property(x => x.PasswordHash).IsModified = false;
        }

        Context.Entry(entity).Property(x => x.OrganizationID).IsModified = false;

        Context.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<AdminEntity?> Create(AdminEditModel editModel)
    {
        var entity = Mapper.Map<AdminEntity>(editModel);
        // TODO проверки

        if (!string.IsNullOrWhiteSpace(editModel.Password))
        {
            entity.PasswordHash = editModel.Password.ToHashSHA256();
        }
        else
        {
            return null;
        }

        Context.Add((object)entity);

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await Context.Admins.FindAsync(id);
        if (entity == null)
        {
            return false;
        }
        

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return true;
    }
}