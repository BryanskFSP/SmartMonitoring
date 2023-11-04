using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Extensions;

namespace SmartMonitoring.Server.Services;

public class InviteService
{
    private SMContext Context;
    private IMapper Mapper;
    private Random rnd;
    private const string LETTERS = "QWERTYUPASDFGHJKLZXCVBNM";
    private const string NUMBERS = "23456789";

    public InviteService(SMContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
        rnd = new Random();
    }

    public List<InviteEntity> GetAll()
    {
        return Context.Invites.AsNoTracking().ToList();
    }

    public string GetAllHash()
    {
        return GetAll().ToHashSHA256();
    }

    public List<InviteEntity> GetAllFull()
    {
        return Context.Invites.AsNoTracking()
            .ToList();
    }

    public string GetAllFullHash()
    {
        return GetAllFull().ToHashSHA256();
    }

    public async Task<InviteEntity?> GetByID(Guid id)
    {
        return await Context.Invites.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDHash(Guid id)
    {
        return (await GetByID(id)).ToHashSHA256();
    }

    public async Task<InviteEntity?> GetByIDFull(Guid id)
    {
        return await Context.Invites.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<string> GetByIDFullHash(Guid id)
    {
        return (await GetByIDFull(id)).ToHashSHA256();
    }

    private async Task<string> GetCode()
    {
        var res = new StringBuilder();

        while (res.Length != 7)
        {
            var rand = rnd.Next(0, 1) == 0 ? false:true;
            if (rand)
            {
                res.Append(LETTERS[rnd.Next(0, LETTERS.Length - 1)]);
            }
            else
            {
                res.Append(NUMBERS[rnd.Next(0, NUMBERS.Length - 1)]);
            }
        }

        return res.ToString();
    }


    public async Task<InviteEntity> GetByCode(string code)
    {
        return Context.Invites.AsNoTracking().FirstOrDefault(x => x.Code == code);
    }
    
    public async Task<InviteEntity?> Update(Guid id, InviteEditModel editModel)
    {
        var entity = Mapper.Map<InviteEntity>(editModel);
        entity.ID = id;

        // TODO проверки

        Context.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;
        Context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
        Context.Entry(entity).Property(x => x.Code).IsModified = false;
        Context.Entry(entity).Property(x => x.OrganizationID).IsModified = false;
        Context.Entry(entity).Property(x => x.OrganizationID).IsModified = false;

        await Context.SaveChangesAsync();

        return entity;
    }

    
    public async Task<InviteEntity?> Create()
    {
        var entity = new InviteEntity();
        var code = await GetCode();
        // TODO проверки
        
        Context.Add((object)entity);

        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await Context.Invites.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return true;
    }
    
    
}