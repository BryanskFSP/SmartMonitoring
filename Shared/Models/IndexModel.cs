namespace SmartMonitoring.Shared.Models;

public class IndexModel
{
    public string RelName { get; set; }
    public long SeqScan { get; set; }
    public long IdxScan { get; set; }
    public long IndexStat { get; set; }
}

public class OutdatedIndexModel
{
    public string Indexrelname { get; set; }
    public string Relname { get; set; }
    public long Stats { get; set; }
}