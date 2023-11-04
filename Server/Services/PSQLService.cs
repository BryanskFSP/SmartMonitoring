using System.Text;
using FormatWith;
using Npgsql;
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

        using var connection = GetConnection(dataBase);
        await connection.OpenAsync();

        var sql = $@"SELECT * FROM sys_caching_ratio()";

        try
        {
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

        using var connection = GetConnection(dataBase);
        await connection.OpenAsync();

        var sql = $@"SELECT * FROM sys_caching_indexes_ratio()";

        try
        {
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

        using var connection = GetConnection(dataBase);
        await connection.OpenAsync();

        var sql = $@"SELECT * FROM {(type == MemoryType.HDD ? "sys_df()" : "sys_free()")}";

        try
        {
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
            Console.WriteLine(e);
            res.Status = false;
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
    public async Task ClearSpace(Guid dbID)
    {
        var res = new ServiceResponse<string>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            return;
        }

        using var connection = GetConnection(dataBase);
        await connection.OpenAsync();

        var sql = $"SELECT sys_clear()";

        using var cmd = new NpgsqlCommand(sql, connection);

        var result = cmd.ExecuteScalar().ToString();

        await connection.CloseAsync();
    }
    
    /// <summary>
    /// Create infitity loop
    /// </summary>
    /// <param name="dbID">DB ID</param>
    public async Task CreateInfinityLoop(Guid dbID)
    {
        var res = new ServiceResponse<MemoryInfoModel>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            return;
        }

        using var connection = GetConnection(dataBase);
        await connection.OpenAsync();

        var sql = $"SELECT startinfinitytimework();";

        using var cmd = new NpgsqlCommand(sql, connection);

        cmd.ExecuteNonQuery();

        await connection.CloseAsync();
    }

    /// <summary>
    /// Get activity process in DB.
    /// </summary>
    /// <param name="dbID">DB ID.</param>
    /// <returns>List of Processes.</returns>
    public async Task<List<PGStatActivityModel>> GetModelsActive(Guid dbID)
    {
        var res = new ServiceResponse<MemoryInfoModel>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            return null;
        }

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
        return ress;
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
            return null;
        }

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

    public async Task<ServiceResponse<string>> CreateFunctions(Guid dbID)
    {
        var res = new ServiceResponse<string>();
        var dataBase = await DBService.GetByID(dbID);
        if (dataBase == null)
        {
            return null;
        }

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
}