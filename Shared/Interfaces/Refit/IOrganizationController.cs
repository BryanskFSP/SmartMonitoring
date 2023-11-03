using Refit;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared.Interfaces.Refit;

public interface IOrganizationController
{
    [Get("/api/Organization")]
    Task<List<OrganizationViewModel>> GetAll();
    
    [Get("/api/Organization/full")]
    Task<List<OrganizationViewModel>> GetFull();
    
    [Get("/api/Organization/{id}")]
    Task<OrganizationViewModel> GetByID(Guid id);
    
    [Post("/api/Organization")]
    Task<OrganizationViewModel> Create(OrganizationEditModel Organization);
    
    [Put("/api/Organization/{id}")]
    Task<OrganizationViewModel> Update(Guid id, OrganizationEditModel Organization);
    
    [Delete("/api/Organization/{id}")]
    Task Delete(Guid id);
}