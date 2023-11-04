using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Services;

public class PSQLCheckerService
{
    private PSQLService PsqlService;
    private LogService LogService;

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
            Value = 20
        }
    };

    public PSQLCheckerService(PSQLService psqlService, LogService logService)
    {
        PsqlService = psqlService;
        LogService = logService;
    }

    /// <summary>
    /// Check states.
    /// </summary>
    /// <param name="entity">DataBase entity.</param>
    public async Task CheckState(DataBaseEntity entity)
    {
        var states = await PsqlService.GetModelsActive(entity.ID);
        foreach (var state in states)
        {
            var time = Values.FirstOrDefault(x => x.Type == ReferenceType.ProcessTimeInSeconds).Value;
            var now = DateTime.Now;
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
    }

    /// <summary>
    /// Check memory in DataBase.
    /// </summary>
    /// <param name="memoryType">MemoryType</param>
    /// <param name="entity">DataBase entity.</param>
    /// <returns>MemoryInfoModel.</returns>
    public async Task<ServiceResponse<MemoryInfoModel>> CheckMemory(MemoryType memoryType, DataBaseEntity entity)
    {
        var res = new ServiceResponse<MemoryInfoModel>();
        if (memoryType == MemoryType.HDD)
        {
            var hdd = await PsqlService.GetMemoryInfo(entity.ID, MemoryType.HDD);
            res = hdd;
            
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
                    Description = $"На сервере свободно менее {procent.ToString()} процентов памяти!",
                };
                await LogService.Add(log);
            }
        }
        else if (memoryType == MemoryType.RAM)
        {
            var ram = await PsqlService.GetMemoryInfo(entity.ID, MemoryType.RAM);
            res = ram;
        }

        return res;
    }

    /// <summary>
    /// Checking Caching Ratio by DataBase.
    /// </summary>
    /// <param name="entity">DataBase entity.</param>
    public async Task CheckingCachingRatio(DataBaseEntity entity)
    {
        var cachingRatioValue = Values.FirstOrDefault(x => x.Type == ReferenceType.CachingRatio).Value;
        var cachingRatio = await PsqlService.GetCachingRatio(entity.ID);
        if (cachingRatioValue >= cachingRatio.Data)
        {
            var log = new LogEditModel
            {
                LogType = LogType.Error,
                Action = ActionType.CachingRatio,
                OrganizationID = entity.OrganizationID,
                DataBaseID = entity.ID,
                Description = $"На сервере плохо с кэшированием: {cachingRatio.Data} процентов!",
            };
            await LogService.Add(log);
        }
    }

    /// <summary>
    /// Checking Caching Indexes Ratio in DataBase.
    /// </summary>
    /// <param name="entity">DataBase entity.</param>
    public async Task CheckingCachingIndexesRatio(DataBaseEntity entity)
    {
        var cachingRatioIndexesValue =
            Values.FirstOrDefault(x => x.Type == ReferenceType.CachingIndexesRatio).Value;

        var cachingIndexesRatio = await PsqlService.GetCachingIndexesRatio(entity.ID);

        if (cachingIndexesRatio.Data <= cachingRatioIndexesValue)
        {
            var log = new LogEditModel
            {
                LogType = LogType.Error,
                Action = ActionType.CachingIndexRatio,
                OrganizationID = entity.OrganizationID,
                DataBaseID = entity.ID,
                Description = $"На сервере плохо с кэшированием индексов: {cachingIndexesRatio.Data} процентов!",
            };
            await LogService.Add(log);
        }
    }
}