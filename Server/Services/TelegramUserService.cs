using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Extensions;
using SmartMonitoring.Shared.Interfaces;

namespace SmartMonitoring.Server.Services;

public class TelegramUserService : IService<TelegramUserEntity, TelegramUserEditModel, long>
{
    private SMContext Context;
    private IMapper Mapper;

    public TelegramUserService(SMContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public List<TelegramUserEntity> GetAll()
    {
        return Context.TelegramUsers.AsNoTracking().ToList();
    }

    public string GetAllHash()
    {
        return GetAll().ToHashSHA256();
    }

    public List<TelegramUserEntity> GetAllFull()
    {
        return Context.TelegramUsers.AsNoTracking()
            .ToList();
    }

    public string GetAllFullHash()
    {
        return GetAllFull().ToHashSHA256();
    }

    public async Task<TelegramUserEntity?> GetByID(long id)
    {
        return await Context.TelegramUsers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TelegramID == id);
    }

    public async Task<string> GetByIDHash(long id)
    {
        return (await GetByID(id)).ToHashSHA256();
    }

    public async Task<TelegramUserEntity?> GetByIDFull(long id)
    {
        return await Context.TelegramUsers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TelegramID == id);
    }

    public async Task<string> GetByIDFullHash(long id)
    {
        return (await GetByIDFull(id)).ToHashSHA256();
    }

    public async Task<TelegramUserEntity?> Update(long id, TelegramUserEditModel editModel)
    {
        var entity = Mapper.Map<TelegramUserEntity>(editModel);
        entity.TelegramID = id;

        // TODO проверки

        Context.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;
        Context.Entry(entity).Property(x => x.MetaInfo).IsModified = false;

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<TelegramUserEntity?> Create(TelegramUserEditModel editModel)
    {
        var entity = Mapper.Map<TelegramUserEntity>(editModel);
        // TODO проверки

        var check = Context.TelegramUsers.FirstOrDefault(x => x.TelegramID == editModel.TelegramID);
        if (check != null)
        {
            //todo переписать
            return check;
        }

        Context.Add((object)entity);

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await Context.TelegramUsers.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return true;
    }
}