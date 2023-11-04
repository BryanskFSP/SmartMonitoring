namespace SmartMonitoring.Shared.Models;

public class PSQLLockModel
{
    public string LockedItem { get; set; }
    public long WarningDuration { get; set; }
    public int BlockedPID { get; set; }
    public string BlockedQuery { get; set; }
    public string BlockedMode { get; set; }
}