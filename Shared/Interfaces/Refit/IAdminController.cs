using Refit;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared.Interfaces.Refit;

public interface IAdminController
{
    [Get("/api/Admin")]
    Task<List<AdminViewModel>> GetAll();
    
    [Get("/api/Admin/full")]
    Task<List<AdminViewModel>> GetFull();
    
    [Get("/api/Admin/{id}")]
    Task<AdminViewModel> GetByID(Guid id);
    
    [Post("/api/Admin")]
    Task<AdminViewModel> Create(AdminEditModel Admin);
    
    [Put("/api/Admin/{id}")]
    Task<AdminViewModel> Update(Guid id, AdminEditModel Admin);
    
    [Delete("/api/Admin/{id}")]
    Task Delete(Guid id);
}