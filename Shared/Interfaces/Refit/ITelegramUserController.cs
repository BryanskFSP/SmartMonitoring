using Refit;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared.Interfaces.Refit;

public interface ITelegramUserController
{
    [Get("/api/TelegramUser")]
    Task<List<TelegramUserViewModel>> GetAll();
    
    [Get("/api/TelegramUser/full")]
    Task<List<TelegramUserViewModel>> GetFull();
    
    [Get("/api/TelegramUser/{id}")]
    [Headers("Authorization: Bearer")]
    Task<TelegramUserViewModel> GetByID(long id);
    
    [Post("/api/TelegramUser")]
    [Headers("Authorization: Bearer")]
    Task<TelegramUserViewModel> Create(TelegramUserEditModel TelegramUser);
    
    [Put("/api/TelegramUser/{id}")]
    [Headers("Authorization: Bearer")]
    Task<TelegramUserViewModel> Update(long id, TelegramUserEditModel TelegramUser);
    
    [Delete("/api/TelegramUser/{id}")]
    [Headers("Authorization: Bearer")]
    Task Delete(long id);
}