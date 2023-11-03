// using Microsoft.Extensions.Diagnostics.HealthChecks;
// using Npgsql;
//
// namespace SmartMonitoring.Server.Services;
//
// public class PSQLHealthCheck: IHealthCheck
// {
//     private readonly string _connectionString;
//     
//     public PSQLHealthCheck()
//     {
//         _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
//     }
//     
//     public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
//     {
//         try
//         {
//             using var sqlConnection = new NpgsqlConnection(_connectionString);
//
//              sqlConnection.OpenAsync(cancellationToken);
//
//             using var command = sqlConnection.CreateCommand();
//             command.CommandText = "SELECT 1";
//
//             await command.ExecuteScalarAsync(cancellationToken);
//
//             return HealthCheckResult.Healthy();
//         }
//         catch(Exception ex)
//         {
//             return HealthCheckResult.Unhealthy(
//                 context.Registration.FailureStatus,
//                 exception: ex);
//         }
//     }
// }