using Refit;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Shared.Interfaces;

public interface IReferenceValueController
{
    [Get("/api/ReferenceValue")]
    Task<ServiceResponse<List<ReferenceValueModel>>> GetAll();

    [Get("/api/ReferenceValue/{type}")]
    Task<ReferenceValueModel> GetByType(ReferenceType type);

    [Post("/api/ReferenceValue")]
    Task Set([Body] ReferenceValueModel model);
}