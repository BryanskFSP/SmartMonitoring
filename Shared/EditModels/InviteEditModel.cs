
namespace SmartMonitoring.Shared.EditModels;

public class InviteEditModel
{
    /// <summary>
    /// Количество разрешённого использования.
    /// </summary>
    public int UseCount { get; set; }

    /// <summary>
    /// Количество использований.
    /// </summary>
    public int UsedCount { get; set; }

    /// <summary>
    /// Код.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Дата время создания.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Дата время окончания использования.
    /// </summary>
    public DateTime ExpireAt { get; set; } = DateTime.Now.AddDays(1);

    /// <summary>
    /// ID организации.
    /// </summary>
    public Guid OrganizationID { get; set; }
}