using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;
using SmartMonitoring.Client.HubClients;
using SmartMonitoring.Shared;
using SmartMonitoring.Shared.Interfaces;
using SmartMonitoring.Shared.Interfaces.Refit;

namespace SmartMonitoring.Client.Providers;

public class RefitProvider
{
    public static void AddRefitInterfaces(IServiceCollection services, Uri url)
    {
        // Initialize AutoMapper.
        services.AddAutoMapper(typeof(Mappings));

        // Add services to the container.

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        serializerOptions.PropertyNameCaseInsensitive = true;
        serializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        serializerOptions.Converters.Add(new ObjectToInferredTypesConverter());
        serializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));

        var serializer = new SystemTextJsonContentSerializer(serializerOptions);

        var refitSettings = new RefitSettings()
        {
            ContentSerializer = serializer
        };

        var apiUrl = url;

        services.AddRefitClient<ITelegramUserController>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = apiUrl);
        
        services.AddRefitClient<IOrganizationController>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = apiUrl);
        
        services.AddRefitClient<IAdminController>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = apiUrl);
        
        services.AddRefitClient<IDataBaseController>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = apiUrl);
        
        services.AddRefitClient<ILogController>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = apiUrl);
        
        services.AddRefitClient<IWikiController>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = apiUrl);
        
        services.AddRefitClient<IInviteController>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = apiUrl);
        
        services.AddRefitClient<IReferenceValueController>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = apiUrl);  
        
        services.AddScoped<LogHubClient>(x => new(apiUrl));

    }
}