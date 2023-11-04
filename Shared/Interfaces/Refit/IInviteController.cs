using Refit;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared.Interfaces.Refit;

public interface IInviteController
{
    [Get("/api/Invite")]
    Task<List<InviteViewModel>> GetAll();
    
    [Get("/api/Invite/full")]
    Task<List<InviteViewModel>> GetFull();
    
    [Get("/api/Invite/{id}")]
    Task<InviteViewModel> GetByID(Guid id);
    
    [Post("/api/Invite")]
    Task<InviteViewModel> Create(InviteEditModel Invite);
    
    [Put("/api/Invite/{id}")]
    Task<InviteViewModel> Update(Guid id, InviteEditModel Invite);
    
    [Delete("/api/Invite/{id}")]
    Task Delete(Guid id);
}