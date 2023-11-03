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

    private List<DataBaseEntity> DataBases = new();
    private List<TelegramUserEntity> TelegramUsers = new();

    private static readonly List<ReferenceValueEntity> Values = new()
    {
        new()
        {
            Type = ReferenceType.Free,
            Value = 75
        },
        new()
        {
            Type = ReferenceType.Df,
            Value = 15,
        },
        new()
        {
            Type = ReferenceType.CachingRatio,
            Value = 90
        },
        new()
        {
            Type = ReferenceType.CachingIndexesRatio,
            Value = 90
        },
        new()
        {
            Type = ReferenceType.ProcessTimeInSeconds,
            Value = 60
        }
    };

    public CheckerJob(PSQLService psqlService, DataBaseService dbService, LogService logService)
    {
        PsqlService = psqlService;
        DBService = dbService;
        LogService = logService;
    }

    private async Task CheckState(DateTime now, PGStatActivityModel state, DataBaseEntity entity)
    {
        var time = Values.FirstOrDefault(x => x.Type == ReferenceType.ProcessTimeInSeconds).Value;
        if ((now - state.BackendStart).TotalSeconds >= ((int)time))
        {
            var log = new LogEditModel
            {
                LogType = LogType.Error,
                Action = ActionType.KillInfinityLoop,
                OrganizationID = entity.OrganizationID,
                DataBaseID = entity.ID,
                Description = "Сессия длится более 120 секунд. Время убивать!",
                EntityID = state.PID.ToString()
            };
            await LogService.Add(log);
        }
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
            foreach (var state in states)
            {
                var time = Values.FirstOrDefault(x => x.Type == ReferenceType.ProcessTimeInSeconds).Value;
                if ((now - state.BackendStart).TotalSeconds >= ((int)time))
                {
                    var log = new LogEditModel
                    {
                        LogType = LogType.Error,
                        Action = ActionType.KillInfinityLoop,
                        OrganizationID = entity.OrganizationID,
                        DataBaseID = entity.ID,
                        Description = "Сессия длится более 120 секунд. Время убивать!",
                        EntityID = state.PID.ToString()
                    };
                    await LogService.Add(log);
                }
            }

            var space = await PsqlService.GetMemoryInfo(entity.ID, MemoryType.RAM);
            var hdd = await PsqlService.GetMemoryInfo(entity.ID, MemoryType.HDD);
            Console.WriteLine(space.Data.UseProcent);

            var procentValue = Values.FirstOrDefault(x => x.Type == ReferenceType.Df).Value;

            var procent = int.Parse(hdd.Data.UseProcent.Replace("%", ""));
            if (procent <= procentValue)
            {
                var log = new LogEditModel
                {
                    LogType = LogType.Error,
                    Action = ActionType.NoSpace,
                    OrganizationID = entity.OrganizationID,
                    DataBaseID = entity.ID,
                    Description = $"На сервере свободно менее {procent} процентов памяти!",
                };
                await LogService.Add(log);
            }

            var cachingRatioValue = Values.FirstOrDefault(x => x.Type == ReferenceType.CachingRatio).Value;
            var cachingRatioIndexesValue =
                Values.FirstOrDefault(x => x.Type == ReferenceType.CachingIndexesRatio).Value;

            var cachingRatio = await PsqlService.GetCachingRatio(entity.ID);
            var cachingIndexesRatio = await PsqlService.GetCachingIndexesRatio(entity.ID);

            if (cachingRatioValue >= cachingRatio.Data)
            {
                var log = new LogEditModel
                {
                    LogType = LogType.Error,
                    Action = ActionType.CachingRatio,
                    OrganizationID = entity.OrganizationID,
                    DataBaseID = entity.ID,
                    Description = $"На сервере плохо с кэшированием: {cachingRatio} процентов!",
                };
                await LogService.Add(log);
            }


            if (cachingIndexesRatio.Data >= cachingRatioIndexesValue)
            {
                var log = new LogEditModel
                {
                    LogType = LogType.Error,
                    Action = ActionType.CachingIndexRatio,
                    OrganizationID = entity.OrganizationID,
                    DataBaseID = entity.ID,
                    Description = $"На сервере плохо с кэшированием индексов: {cachingRatio} процентов!",
                };
                await LogService.Add(log);
            }
        }

        // await Task.WhenAll(tasks);
    }
}