namespace SmartMonitoring.Shared.Models;

/// <summary>
/// https://postgrespro.ru/docs/postgresql/14/monitoring-stats#MONITORING-PG-STAT-ACTIVITY-VIEW
/// </summary>
public class PGStatActivityModel
{
    public long? DatID { get; set; }
    public string DatName { get; set; }
    public long PID { get; set; }
    public long? LeaderPID { get; set; }
    public long? UseSysID { get; set; }
    public string Usename { get; set; }
    public string? ApplicationName { get; set; }
    public string? ClientAddr { get; set; }
    public string? ClientHostname { get; set; }
    public int? ClientPort { get; set; }
    public DateTime BackendStart { get; set; }
    public string? wait_event_type  { get; set; }
    public string? wait_event  { get; set; }
    public string state  { get; set; }
    public string? query_id  { get; set; }
    public string? query  { get; set; }
    public string? backend_type  { get; set; }
}