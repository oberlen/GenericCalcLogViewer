using System.Data.SqlClient;
using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// שירות אחזור לוגים ממסד נתונים SQL Server
/// </summary>
public class DatabaseLogService : IDatabaseLogService
{
    private readonly ILogger<DatabaseLogService> _logger;
    private const int MaxRecords = 10000;

    public DatabaseLogService(ILogger<DatabaseLogService> logger)
    {
        _logger = logger;
    }

    public async Task<List<LogEntry>> GetLogsAsync(
        int caseNumber, 
        DateTime fromDate, 
        DateTime toDate, 
        string connectionString)
    {
        var logs = new List<LogEntry>();

        var query = @"
            SELECT TOP (@MaxRecords)
                ms_merkaz_req,
                op_time,
                log_level,
                message
            FROM gvia.srv_GenericCalc_log WITH (NOLOCK)
            WHERE ms_merkaz_req = @CaseNumber
                AND op_time >= @FromDate
                AND op_time <= @ToDate
            ORDER BY op_time ASC";

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaxRecords", MaxRecords);
            command.Parameters.AddWithValue("@CaseNumber", caseNumber);
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                logs.Add(new LogEntry
                {
                    Timestamp = reader.GetDateTime(reader.GetOrdinal("op_time")),
                    Source = "DB",
                    Level = reader.GetString(reader.GetOrdinal("log_level")),
                    Message = reader.GetString(reader.GetOrdinal("message")),
                    CaseNumber = reader.GetInt32(reader.GetOrdinal("ms_merkaz_req")).ToString()
                });
            }

            _logger.LogInformation("Retrieved {Count} logs from database for case {CaseNumber}", 
                logs.Count, caseNumber);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error while retrieving logs for case {CaseNumber}", caseNumber);
            throw new InvalidOperationException($"Failed to retrieve logs from database: {ex.Message}", ex);
        }

        return logs;
    }
}
