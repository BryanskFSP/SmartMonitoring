using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Serilog;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Services;

public class ReferenceValuesService
{
    private IDistributedCache cache;

    public ReferenceValuesService(IDistributedCache cache)
    {
        this.cache = cache;
    }

    private HashSet<ReferenceValueModel> Values = new()
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


    public async Task<ServiceResponse<List<ReferenceValueModel>>> GetAll()
    {
        var res = new ServiceResponse<List<ReferenceValueModel>>();
        res.Data = new();
        foreach (var value in Values)
        {
            res.Data.Add(value);
        }

        return res;
    }

    public async Task<ReferenceValueModel> GetValue(ReferenceType type)
    {
        try
        {
            var res = await cache.GetStringAsync(type.ToString());
            if (res == null)
            {
                await InitValues();
                return Values.FirstOrDefault(x => x.Type == type);
            }

            var ent = JsonConvert.DeserializeObject<ReferenceValueModel>(res);
            return ent;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in get in Redis");
            return Values.FirstOrDefault(x => x.Type == type);
        }
    }

    private async Task InitValues()
    {
        foreach (var valueEntity in Values)
        {
            try
            {
                await cache.SetStringAsync(valueEntity.Type.ToString(), JsonConvert.SerializeObject(valueEntity),
                    new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365)
                    });
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in init in Redis");
            }
        }
    }

    public async Task<decimal> GetValueScalar(ReferenceType type)
    {
        return (await GetValue(type)).Value;
    }

    public async Task SettingValueElement(ReferenceValueModel valueModel)
    {
        try
        {
            Values.RemoveWhere(x => x.Type == valueModel.Type);
            Values.Add(valueModel);
            await cache.SetStringAsync(valueModel.Type.ToString(), JsonConvert.SerializeObject(valueModel),
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365)
                });
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in init in Redis");
        }

        Values.Add(valueModel);
    }
}