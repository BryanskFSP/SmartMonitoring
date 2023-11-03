namespace SmartMonitoring.Shared.Models;

/// <summary>
/// Model for return result from service/endpoint.
/// </summary>
/// <typeparam name="T">Output entity.</typeparam>
public class ServiceResponse<T>
{
    /// <summary>
    /// Status of Response.
    /// </summary>
    public bool Status { get; set; }
    /// <summary>
    /// Name of Status Response (Success, Error, Verbose, etc.)
    /// </summary>
    public string Name { get; set; } = "Ошибка не указана.";
    
    /// <summary>
    /// Output entity.
    /// </summary>
    public T? Data { get; set; }
}


public class MemoryInfoModel
{
    public string Total { get; set; }
    public string Used { get; set; }
    public string Avail { get; set; }
    public string? UseProcent { get; set; }
    public MemoryType Type { get; set; }
}

public enum MemoryType
{
    RAM,
    HDD
}