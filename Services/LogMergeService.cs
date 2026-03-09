using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// שירות מיזוג ומיון לוגים ממקורות שונים
/// </summary>
public class LogMergeService : ILogMergeService
{
    private readonly ILogger<LogMergeService> _logger;

    public LogMergeService(ILogger<LogMergeService> logger)
    {
        _logger = logger;
    }

    public List<LogEntry> MergeAndSort(List<LogEntry> dbLogs, List<LogEntry> fileLogs)
    {
        var allLogs = new List<LogEntry>();
        
        if (dbLogs != null)
        {
            allLogs.AddRange(dbLogs);
        }
        
        if (fileLogs != null)
        {
            allLogs.AddRange(fileLogs);
        }

        // Sort by Timestamp ascending, then by Source (FILE before DB for same timestamp)
        var sortedLogs = allLogs
            .OrderBy(log => log.Timestamp)
            .ThenBy(log => log.Source == "FILE" ? 0 : 1)
            .ToList();

        _logger.LogInformation("Merged and sorted {TotalCount} logs ({DbCount} from DB, {FileCount} from files)",
            sortedLogs.Count, dbLogs?.Count ?? 0, fileLogs?.Count ?? 0);

        return sortedLogs;
    }
}
