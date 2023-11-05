using System.Text;
using FormatWith;
using Npgsql;
using Serilog;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Services;

public class PSQLService
{
    private DataBaseService DBService;

    private const string ConnectStringPlaceholder =
        "Host={HOST};Database={DATABASE};Username={USER};Password={PASSWORD}";

    public PSQLService(DataBaseService dbService)
    {
        DBService = dbService;
    }
    
    /// <summary>
    /// Run script.
    /// </summary>
    /// <param name="dbID">DB ID</param>
    /// <param name="script">Script.</param>
    public async Task<ServiceResponse<string>> RunScript(Guid dbID, string script)
    {
        var res = new ServiceResponse<string>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();
            
            using var cmd = new NpgsqlCommand(script, connection);

            var reader = await cmd.ExecuteReaderAsync();
            var ress = new StringBuilder();
            while (reader.Read())
            {
                try
                {
                    ress.AppendLine(reader.GetString(0));
                }
                catch (Exception e)
                {
                                    
                }
            }

            res.Data = ress.ToString();

            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }

        return res;
    }

    /// <summary>
    /// Get DB Caching ratio.
    /// </summary>
    /// <param name="dataBaseID">Database ID.</param>
    /// <returns></returns>
    public async Task<ServiceResponse<decimal>> GetCachingRatio(Guid dataBaseID)
    {
        var res = new ServiceResponse<decimal>();
        var dataBase = await DBService.GetByID(dataBaseID);
        if (dataBase == null)
        {
            return null;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var sql = $@"SELECT * FROM sys_caching_ratio()";

            using var cmd = new NpgsqlCommand(sql, connection);

            using var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var data = dataReader.GetDecimal(0);
                res.Data = data;
            }

            dataReader.Close();
            await connection.CloseAsync();
            res.Status = true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetCachingRatio");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }

        return res;
    }

    /// <summary>
    /// Get DB Caching indexes ratio.
    /// </summary>
    /// <param name="dataBaseID">Database ID.</param>
    /// <returns></returns>
    public async Task<ServiceResponse<decimal>> GetCachingIndexesRatio(Guid dataBaseID)
    {
        var res = new ServiceResponse<decimal>();
        var dataBase = await DBService.GetByID(dataBaseID);
        if (dataBase == null)
        {
            return null;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var sql = $@"SELECT * FROM sys_caching_indexes_ratio()";

            using var cmd = new NpgsqlCommand(sql, connection);

            using var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var data = dataReader.GetDecimal(0);
                res.Data = data;
            }

            dataReader.Close();
            await connection.CloseAsync();
            res.Status = true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetCachingIndexesRatio");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }

        return res;
    }

    /// <summary>
    /// Get memory info in DB.
    /// </summary>
    /// <param name="dataBaseID">DB ID.</param>
    /// <param name="type">Memory type.</param>
    /// <returns></returns>
    public async Task<ServiceResponse<MemoryInfoModel>> GetMemoryInfo(Guid dataBaseID, MemoryType type)
    {
        var res = new ServiceResponse<MemoryInfoModel>();
        var dataBase = await DBService.GetByID(dataBaseID);
        if (dataBase == null)
        {
            return null;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var sql = $@"SELECT * FROM {(type == MemoryType.HDD ? "sys_df()" : "sys_free()")}";

            using var cmd = new NpgsqlCommand(sql, connection);

            using var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var data = dataReader.GetString(0).Split(',');
                res.Data = new();
                res.Data.Type = type;
                if (type == MemoryType.HDD)
                {
                    res.Data.Total = data[2];
                    res.Data.Used = data[3];
                    res.Data.Avail = data[4];
                    res.Data.UseProcent = data[5];
                }
                else
                {
                    res.Data.Total = data[1];
                    res.Data.Used = data[2];
                    res.Data.Avail = data[3];
                    res.Data.UseProcent = null;
                }
            }

            dataReader.Close();
            await connection.CloseAsync();
            res.Status = true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }

        return res;
    }

    /// <summary>
    /// Get connection to DB.
    /// </summary>
    /// <param name="entity">DB entity.</param>
    /// <returns></returns>
    private NpgsqlConnection GetConnection(DataBaseEntity entity)
    {
        return new NpgsqlConnection(ConnectStringPlaceholder.FormatWith(new
        {
            HOST = entity.Host,
            DATABASE = entity.Database,
            USER = entity.User,
            PASSWORD = entity.Password
        }));
    }

    /// <summary>
    /// Kill process in DB>
    /// </summary>
    /// <param name="dbID">DB ID</param>
    /// <param name="pid">PID.</param>
    public async Task KillProcess(Guid dbID, string pid)
    {
        var res = new ServiceResponse<MemoryInfoModel>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            return;
        }

        using var connection = GetConnection(dataBase);
        await connection.OpenAsync();

        var sql = $"SELECT pg_cancel_backend({pid})";

        using var cmd = new NpgsqlCommand(sql, connection);

        cmd.ExecuteNonQuery();

        await connection.CloseAsync();
    }

    /// <summary>
    /// Clear space in DB.
    /// </summary>
    /// <param name="dbID">DB ID</param>
    public async Task<ServiceResponse<string>> ClearSpace(Guid dbID)
    {
        var res = new ServiceResponse<string>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var sql = $"SELECT sys_clear()";

            using var cmd = new NpgsqlCommand(sql, connection);

            var result = cmd.ExecuteScalar().ToString();

            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }

        return res;
    }

    /// <summary>
    /// Clear space by vacuum in DB.
    /// </summary>
    /// <param name="dbID">DB ID</param>
    public async Task<ServiceResponse<string>> ClearSpaceByVacuum(Guid dbID)
    {
        var res = new ServiceResponse<string>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var sql = $"VACUUM FULL;";

            using var cmd = new NpgsqlCommand(sql, connection);

            var result = cmd.ExecuteScalar().ToString();

            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }

        return res;
    }

    /// <summary>
    /// Create infitity loop
    /// </summary>
    /// <param name="dbID">DB ID</param>
    public async Task<ServiceResponse<string>> CreateInfinityLoop(Guid dbID)
    {
        var res = new ServiceResponse<string>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var sql = $"SELECT startinfinitytimework();";

            using var cmd = new NpgsqlCommand(sql, connection);

            await Task.Run(async () => await (await cmd.ExecuteReaderAsync()).CloseAsync());

            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }

        return res;
    }

    /// <summary>
    /// Get activity process in DB.
    /// </summary>
    /// <param name="dbID">DB ID.</param>
    /// <returns>List of Processes.</returns>
    public async Task<ServiceResponse<List<PGStatActivityModel>>> GetModelsActive(Guid dbID)
    {
        var res = new ServiceResponse<List<PGStatActivityModel>>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var ress = new List<PGStatActivityModel>();

            string sql =
                "SELECT pid, datname, usename, state, backend_start\nFROM pg_stat_activity\nWHERE state = 'active'";
            var cmd = new NpgsqlCommand(sql, connection);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var model = new PGStatActivityModel()
                {
                    PID = rdr.GetInt64(0),
                    DatName = rdr.GetString(1),
                    Usename = rdr.GetString(2),
                    state = rdr.GetString(3),
                    BackendStart = rdr.GetDateTime(4),
                };
                ress.Add(model);
            }

            await rdr.CloseAsync();


            sql = "SELECT pg_backend_pid()";
            cmd = new NpgsqlCommand(sql, connection);
            var dr = cmd.ExecuteReader();
            var id = 0;
            while (dr.Read())
            {
                id = dr.GetInt32(0);
            }

            ress.RemoveAll(x => x.PID == id);
            await dr.CloseAsync();

            await connection.CloseAsync();
            res.Data = ress;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }

        return res;
    }

    /// <summary>
    /// Get top Operations In Tables
    /// </summary>
    /// <param name="dbID">DB ID.</param>
    /// <returns>List of Table stats model.</returns>
    public async Task<ServiceResponse<List<TableStatsModel>>> GetTopOperationsInTables(Guid dbID)
    {
        var res = new ServiceResponse<List<TableStatsModel>>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var ress = new List<TableStatsModel>();

            string sql =
                @"SELECT
   relname, 
   n_tup_upd+n_tup_ins+n_tup_del AS operationsAmount
FROM pg_stat_user_tables
ORDER BY operationsAmount DESC;
";
            var cmd = new NpgsqlCommand(sql, connection);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var model = new TableStatsModel()
                {
                    Name = rdr.GetString(0),
                    OperationsCount = rdr.GetInt64(1)
                };
                ress.Add(model);
            }

            await rdr.CloseAsync();

            await connection.CloseAsync();

            res.Data = ress;
            res.Status = true;
            return res;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }
    }

    public async Task<ServiceResponse<string>> CreateFunctions(Guid dbID)
    {
        var res = new ServiceResponse<string>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var sql = new StringBuilder();
            sql.AppendLine(@"CREATE OR REPLACE FUNCTION sys_caching_ratio() RETURNS SETOF numeric
LANGUAGE plpgsql as
$$
BEGIN
    RETURN QUERY SELECT ((sum(heap_blks_hit) / (sum(heap_blks_hit) + sum(heap_blks_read))) * 100) as ratio FROM pg_statio_user_tables;
END;
$$;");

            sql.AppendLine(@"CREATE OR REPLACE FUNCTION sys_caching_indexes_ratio() RETURNS SETOF numeric
LANGUAGE plpgsql as
$$
BEGIN
    RETURN QUERY SELECT (((sum(idx_blks_hit) - sum(idx_blks_read)) / sum(idx_blks_hit)) * 100) as ratio FROM pg_statio_user_indexes;
END;
$$;
");
            sql.AppendLine(@"CREATE OR REPLACE FUNCTION sys_clear() RETURNS SETOF text
LANGUAGE plpgsql as
$$
BEGIN
    CREATE TEMP TABLE IF NOT EXISTS tmp_sys_clear (content text) ON COMMIT DROP;
    COPY tmp_sys_clear FROM PROGRAM 'python3 /cleartrash.py';
    RETURN QUERY SELECT CAST(regexp_split_to_array(content, '\s+') as text) FROM tmp_sys_clear LIMIT 1;
END;
$$;");
            sql.AppendLine(@"");
            sql.AppendLine(@"");

            using var cmd = new NpgsqlCommand(sql.ToString(), connection);

            cmd.ExecuteNonQuery();

            await connection.CloseAsync();

            return res;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }
    }

    /// <summary>
    /// Get blocked processes in DB.
    /// </summary>
    /// <param name="dbID">DB ID.</param>
    /// <returns>List of Processes.</returns>
    public async Task<ServiceResponse<List<PSQLLockModel>>> GetLockProcesses(Guid dbID)
    {
        var res = new ServiceResponse<List<PSQLLockModel>>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var ress = new List<PSQLLockModel>();

            string sql =
                @"SELECT COALESCE(blockingl.relation::regclass::text, blockingl.locktype) AS locked_item,
       now() - blockeda.query_start                                     AS waiting_duration,
       blockeda.pid                                                     AS blocked_pid,
       blockeda.query                                                   AS blocked_query,
       blockedl.mode                                                    AS blocked_mode
FROM pg_locks blockedl
JOIN pg_stat_activity blockeda ON blockedl.pid = blockeda.pid
JOIN pg_locks blockingl ON (blockingl.transactionid = blockedl.transactionid OR
                            blockingl.relation = blockedl.relation AND
                            blockingl.locktype = blockedl.locktype) AND blockedl.pid <> blockingl.pid
JOIN pg_stat_activity blockinga ON blockingl.pid = blockinga.pid AND blockinga.datid = blockeda.datid
WHERE NOT blockedl.granted AND blockinga.datname = current_database();
";
            var cmd = new NpgsqlCommand(sql, connection);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var model = new PSQLLockModel()
                {
                    LockedItem = rdr.GetString(0),
                    WarningDuration = rdr.GetInt64(1),
                    BlockedPID = rdr.GetInt32(2),
                    BlockedQuery = rdr.GetString(3),
                    BlockedMode = rdr.GetString(4),
                };
                ress.Add(model);
            }

            await rdr.CloseAsync();

            await connection.CloseAsync();
            res.Data = ress;
            return res;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }
    }

    /// <summary>
    /// Get indexes stats in DB.
    /// </summary>
    /// <param name="dbID">DB ID.</param>
    /// <returns>List of Indexes.</returns>
    public async Task<ServiceResponse<List<IndexModel>>> GetIndexesStats(Guid dbID)
    {
        var res = new ServiceResponse<List<IndexModel>>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var ress = new List<IndexModel>();

            string sql =
                @"SELECT 
   relname, 
   seq_scan, 
   idx_scan, 
   idx_scan/seq_scan as IndexStat
FROM pg_stat_user_tables
WHERE seq_scan <> 0
ORDER BY IndexStat DESC;
";
            var cmd = new NpgsqlCommand(sql, connection);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var model = new IndexModel()
                {
                    RelName = rdr.GetString(0),
                    SeqScan = rdr.GetInt64(1),
                    IdxScan = rdr.GetInt64(2),
                    IndexStat = rdr.GetInt64(3)
                };
                ress.Add(model);
            }

            await rdr.CloseAsync();

            await connection.CloseAsync();
            res.Data = ress;
            return res;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }
    }

    /// <summary>
    /// Get outdated index stats in DB.
    /// </summary>
    /// <param name="dbID">DB ID.</param>
    /// <returns>List of outdated Indexes.</returns>
    public async Task<ServiceResponse<List<OutdatedIndexModel>>> GetOutdatedIndexesStats(Guid dbID)
    {
        var res = new ServiceResponse<List<OutdatedIndexModel>>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var ress = new List<OutdatedIndexModel>();

            string sql =
                @"SELECT 
   indexrelname, 
   relname, 
   idx_tup_read/idx_tup_fetch as stats
FROM pg_stat_user_indexes
WHERE idx_tup_fetch <> 0
ORDER BY stats DESC;
";
            var cmd = new NpgsqlCommand(sql, connection);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var model = new OutdatedIndexModel()
                {
                    Indexrelname = rdr.GetString(0),
                    Relname = rdr.GetString(1),
                    Stats = rdr.GetInt64(2),
                };
                ress.Add(model);
            }

            await rdr.CloseAsync();

            await connection.CloseAsync();
            res.Data = ress;
            return res;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetMemoryInfo");
            res.Status = false;
            res.Name = e.Message;
            return res;
        }
    }

    /// <summary>
    /// Get DB Wasted bytes.
    /// </summary>
    /// <param name="dataBaseID">Database ID.</param>
    /// <returns></returns>
    public async Task<ServiceResponse<decimal>> GetWastedBytes(Guid dataBaseID)
    {
        var res = new ServiceResponse<decimal>();
        var dataBase = await DBService.GetByID(dataBaseID);
        if (dataBase == null)
        {
            res.Name = "DB not found";
            return res;
        }

        try
        {
            using var connection = GetConnection(dataBase);
            await connection.OpenAsync();

            var sql = $@"SELECT
  CASE WHEN relpages < otta THEN 0 ELSE bs*(sml.relpages-otta)::BIGINT END AS wastedbytes
FROM (
  SELECT
    schemaname, tablename, cc.reltuples, cc.relpages, bs,
    CEIL((cc.reltuples*((datahdr+ma-
      (CASE WHEN datahdr%ma=0 THEN ma ELSE datahdr%ma END))+nullhdr2+4))/(bs-20::float)) AS otta,
    COALESCE(c2.relname,'?') AS iname, COALESCE(c2.reltuples,0) AS ituples, COALESCE(c2.relpages,0) AS ipages,
    COALESCE(CEIL((c2.reltuples*(datahdr-12))/(bs-20::float)),0) AS iotta /* very rough approximation, assumes all cols */
  FROM (
    SELECT
      ma,bs,schemaname,tablename,
      (datawidth+(hdr+ma-(case when hdr%ma=0 THEN ma ELSE hdr%ma END)))::numeric AS datahdr,
      (maxfracsum*(nullhdr+ma-(case when nullhdr%ma=0 THEN ma ELSE nullhdr%ma END))) AS nullhdr2
    FROM (
      SELECT
        schemaname, tablename, hdr, ma, bs,
        SUM((1-null_frac)*avg_width) AS datawidth,
        MAX(null_frac) AS maxfracsum,
        hdr+(
          SELECT 1+count(*)/8
          FROM pg_stats s2
          WHERE null_frac<>0 AND s2.schemaname = s.schemaname AND s2.tablename = s.tablename
        ) AS nullhdr
      FROM pg_stats s, (
        SELECT
          (SELECT current_setting('block_size')::numeric) AS bs,
          CASE WHEN substring(v,12,3) IN ('8.0','8.1','8.2') THEN 27 ELSE 23 END AS hdr,
          CASE WHEN v ~ 'mingw32' THEN 8 ELSE 4 END AS ma
        FROM (SELECT version() AS v) AS foo
      ) AS constants
      GROUP BY 1,2,3,4,5
    ) AS foo
  ) AS rs
  JOIN pg_class cc ON cc.relname = rs.tablename
  JOIN pg_namespace nn ON cc.relnamespace = nn.oid AND nn.nspname = rs.schemaname AND nn.nspname <> 'information_schema'
  LEFT JOIN pg_index i ON indrelid = cc.oid
  LEFT JOIN pg_class c2 ON c2.oid = i.indexrelid
) AS sml
ORDER BY wastedbytes DESC limit 1;
";
            using var cmd = new NpgsqlCommand(sql, connection);

            using var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var data = dataReader.GetDecimal(0);
                res.Data = data;
            }

            dataReader.Close();
            await connection.CloseAsync();
            res.Status = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            res.Status = false;
            return res;
        }

        return res;
    }
}