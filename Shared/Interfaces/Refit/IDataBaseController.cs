using Refit;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared.Interfaces.Refit;

public interface IDataBaseController
{
    [Get("/api/DataBase")]
    Task<List<DataBaseViewModel>> GetAll();
    
    [Get("/api/DataBase/full")]
    Task<List<DataBaseViewModel>> GetFull();
    
    [Get("/api/DataBase/{id}")]
    [Headers("Authorization: Bearer")]
    Task<DataBaseViewModel> GetByID(Guid id);
    
    [Post("/api/DataBase")]
    Task<DataBaseViewModel> Create(DataBaseEditModel DataBase);
    
    [Put("/api/DataBase/{id}")]
    Task<DataBaseViewModel> Update(Guid id, DataBaseEditModel DataBase);
    
    [Delete("/api/DataBase/{id}")]
    Task Delete(Guid id);
}