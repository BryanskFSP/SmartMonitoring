using Microsoft.EntityFrameworkCore;
using SmartMonitoring.Server.Entities;

namespace SmartMonitoring.Server;

public class SMContext : DbContext
{
    public DbSet<AdminEntity> Admins => Set<AdminEntity>();
    public DbSet<DataBaseEntity> DataBases => Set<DataBaseEntity>();
    public DbSet<InviteEntity> Invites => Set<InviteEntity>();
    public DbSet<LogEntity> Logs => Set<LogEntity>();
    public DbSet<OrganizationEntity> Organizations => Set<OrganizationEntity>();
    public DbSet<WikiEntity> Wikies => Set<WikiEntity>();
    public DbSet<WikiSolutionEntity> WikiSolutions => Set<WikiSolutionEntity>();
    public DbSet<TelegramUserEntity> TelegramUsers => Set<TelegramUserEntity>();
    
    public SMContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public SMContext(DbContextOptions<SMContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}