namespace SmartMonitoring.Shared.Models;

/// <summary>
/// Auth Helper model.
/// </summary>
public class AuthHelperModel<T>
{
    /// <summary>
    /// Entity name.
    /// </summary>
    public string Entity { get; set; }

    /// <summary>
    /// Entity JSON.
    /// </summary>
    public T EntityJSON { get; set; }

    /// <summary>
    /// Entity hash.
    /// </summary>
    public string? EntityHash { get; set; }
    
    /// <summary>
    /// Entity ID.
    /// </summary>
    public string EntityID { get; set; }
    
    /// <summary>
    /// Platform ID.
    /// </summary>
    public string? PlatformID { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    public string Token { get; set; }
}