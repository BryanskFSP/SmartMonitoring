using Npgsql;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Server.Services;

public class PSQLService
{
    private SMContext Context;
    private NpgsqlConnection Connection;

    public PSQLService(SMContext context)
    {
        Context = context;
        Connection = new(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
    }

    public async Task KillProcess(long pid)
    {
        await Connection.OpenAsync();

        var sql = $"SELECT pg_cancel_backend({pid})";

        using var cmd = new NpgsqlCommand(sql, Connection);

        var version = cmd.ExecuteScalar().ToString();
        
        await Connection.CloseAsync();
    }

    public async Task<List<PGStatActivityModel>> GetModelsActive()
    {
        var ress = new List<PGStatActivityModel>();

        await Connection.OpenAsync();

        string sql =
            "SELECT pid, datname, usename, state, backend_start\nFROM pg_stat_activity\nWHERE state = 'active'";
        using var cmd = new NpgsqlCommand(sql, Connection);

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
        await Connection.CloseAsync();
        return ress;
    }
}