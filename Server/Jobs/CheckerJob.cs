using Quartz;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Server.Services;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Jobs;

public class CheckerJob : IJob
{
    private PSQLService PsqlService;
    private DataBaseService DBService;
    private PSQLCheckerService PsqlCheckerService;

    private List<DataBaseEntity> DataBases = new();

    public CheckerJob(PSQLService psqlService, DataBaseService dbService,
        PSQLCheckerService psqlCheckerService)
    {
        PsqlService = psqlService;
        DBService = dbService;
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