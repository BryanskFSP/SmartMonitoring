using Refit;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared.Interfaces.Refit;

public interface IWikiController
{
    [Get("/api/Wiki")]
    Task<List<WikiViewModel>> GetAll();
    
    [Get("/api/Wiki/full")]
    Task<List<WikiViewModel>> GetFull();
    
    [Get("/api/Wiki/{id}")]
    [Headers("Authorization: Bearer")]
    Task<WikiViewModel> GetByID(Guid id);
    
    [Post("/api/Wiki")]
    [Headers("Authorization: Bearer")]
    Task<WikiViewModel> Create(WikiEditModel Wiki);
    
    [Put("/api/Wiki/{id}")]
    [Headers("Authorization: Bearer")]
    Task<WikiViewModel> Update(Guid id, WikiEditModel Wiki);
    
    [Delete("/api/Wiki/{id}")]
    [Headers("Authorization: Bearer")]
    Task Delete(Guid id);
}