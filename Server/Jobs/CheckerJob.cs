using Quartz;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Interfaces.Refit;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Jobs;

public class CheckerJob : IJob
{
    private PSQLService PsqlService;
    private DataBaseService DBService;
    private LogService LogService;
    private PSQLCheckerService PsqlCheckerService;

    private List<DataBaseEntity> DataBases = new();
    private List<TelegramUserEntity> TelegramUsers = new();

    public CheckerJob(PSQLService psqlService, DataBaseService dbService, LogService logService,
        PSQLCheckerService psqlCheckerService)
    {
        PsqlService = psqlService;
        DBService = dbService;
        LogService = logService;
        PsqlCheckerService = psqlCheckerService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.Now;
        Console.WriteLine("[{0}] Start checking...", DateTime.Now);
        DataBases = DBService.GetAll();
        List<Task> tasks = new();

        foreach (var entity in DataBases)
        {
            var states = await PsqlService.GetModelsActive(entity.ID);
            await PsqlCheckerService.CheckState(entity);

            await PsqlCheckerService.CheckMemory(MemoryType.HDD, entity);
            await PsqlCheckerService.CheckingCachingRatio(entity);
            await PsqlCheckerService.CheckingCachingIndexesRatio(entity);
        }

        // await Task.WhenAll(tasks);
    }
}