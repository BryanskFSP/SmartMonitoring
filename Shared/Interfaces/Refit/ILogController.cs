using Refit;
using SmartMonitoring.Shared.Models;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared.Interfaces.Refit;

public interface ILogController
{
    [Get("/api/Log")]
    Task<List<LogViewModel>> GetAll();  
    [Get("/api/Log/full")]
    Task<List<LogViewModel>> GetFull();

    [Get("/api/Log/db/{dbid}")]
    Task<List<LogViewModel>> GetByDBID(Guid dbid);
    
    [Post("/api/Log/{id}")]
    Task<ServiceResponse<string>> FixError(Guid id);
    
    [Get("/api/Log/{id}")]
    Task<LogViewModel> GetByID(Guid id);
}