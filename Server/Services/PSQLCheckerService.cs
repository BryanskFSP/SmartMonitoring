using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Services;

/// <summary>
/// Сервис, отвечающий за регистрации отколнений.
/// </summary>
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
        var resp = await PsqlService.GetModelsActive(entity.ID);
        if (resp.Status == false)
        {
            return;
        }

        var states = resp.Data; 
        foreach (var state in states)
        {
            var time = Values.FirstOrDefault(x => x.Type == ReferenceType.ProcessTimeInSeconds).Value;
            var now = DateTime.Now;
            
            var log = new LogEditModel
            {
                LogType = LogType.Verbose,
                Action = ActionType.KillInfinityLoop,
                OrganizationID = entity.OrganizationID,
                DataBaseID = entity.ID,
                EntityID = state.PID.ToString(),
                Entity = ((now - state.BackendStart).TotalSeconds).ToString(),
                Name = "Информация о процессе",
                Description = $"PID {state.PID}"
            };

            if ((now - state.BackendStart).TotalSeconds >= ((int)time))
            {
                log.LogType = LogType.Error;
                log.Description  +=$"\nERROR!\nСессия длится более {log.Entity} секунд. Время убивать!";
            }
            await LogService.Add(log);
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
        var log = new LogEditModel
        {
            LogType = LogType.Verbose,
            Action = ActionType.NoSpace,
            OrganizationID = entity.OrganizationID,
            DataBaseID = entity.ID,
            Name = "Проверка памяти",
            Description = $"Проверка памяти: "
        };

        if (memoryType == MemoryType.HDD)
        {
            var hdd = await PsqlService.GetMemoryInfo(entity.ID, MemoryType.HDD);
            res = hdd;

            var procentValue = Values.FirstOrDefault(x => x.Type == ReferenceType.Df).Value;
            var procent = int.Parse(hdd.Data.UseProcent.Replace("%", ""));
            log.Entity = procent.ToString();
            log.Description += $"{procent.ToString()} процентов";
            if (procent <= procentValue)
            {
                log.LogType = LogType.Error;
                log.Action = ActionType.NoSpace;
                log.Description =
                    $"На сервере свободно менее {procent.ToString()} процентов памяти!";
            }
        }
        else if (memoryType == MemoryType.RAM)
        {
            var ram = await PsqlService.GetMemoryInfo(entity.ID, MemoryType.RAM);
            res = ram;
        }

        await LogService.Add(log);

        res.Status = true;
        return res;
    }

    /// <summary>
    /// Checking Caching Ratio by DataBase.
    /// </summary>
    /// <param name="entity">DataBase entity.</param>
    public async Task<ServiceResponse<decimal>> CheckingCachingRatio(DataBaseEntity entity)
    {
        var cachingRatioValue = Values.FirstOrDefault(x => x.Type == ReferenceType.CachingRatio).Value;
        var cachingRatio = await PsqlService.GetCachingRatio(entity.ID);
        var log = new LogEditModel
        {
            LogType = LogType.Verbose,
            Action = ActionType.CachingRatio,
            OrganizationID = entity.OrganizationID,
            DataBaseID = entity.ID,
            Name = "Кэширование",
            Description = $"{cachingRatio.Data} процентов",
            Entity = cachingRatio.Data.ToString()
        };

        if (cachingRatioValue >= cachingRatio.Data)
        {
            log.LogType = LogType.Error;
            log.Description =
                $"На сервере плохо с кэшированием: {cachingRatio.Data} процентов!";
        }

        await LogService.Add(log);
        return cachingRatio;
    }

    /// <summary>
    /// Checking Caching Indexes Ratio in DataBase.
    /// </summary>
    /// <param name="entity">DataBase entity.</param>
    public async Task<ServiceResponse<decimal>> CheckingCachingIndexesRatio(DataBaseEntity entity)
    {
        var cachingRatioIndexesValue =
            Values.FirstOrDefault(x => x.Type == ReferenceType.CachingIndexesRatio).Value;
        var cachingIndexesRatio = await PsqlService.GetCachingIndexesRatio(entity.ID);

        var log = new LogEditModel
        {
            LogType = LogType.Verbose,
            Action = ActionType.CachingIndexRatio,
            OrganizationID = entity.OrganizationID,
            DataBaseID = entity.ID,
            Name = "Кэширование",
            Description = $"{cachingIndexesRatio.Data} процентов",
            Entity = cachingIndexesRatio.Data.ToString()
        };

        if (cachingIndexesRatio.Data <= cachingRatioIndexesValue)
        {
            log.LogType = LogType.Error;
            log.Description = $"На сервере плохо с кэшированием индексов: {cachingIndexesRatio.Data} процентов!";
        }

        
        await LogService.Add(log);
        return cachingIndexesRatio;
    }
}