using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// שירות תיאום חיפוש לוגים מכל המקורות
/// </summary>
public class LogSearchService : ILogSearchService
{
    private readonly IEnvironmentService _environmentService;
    private readonly IDatabaseLogService _databaseLogService;
    private readonly IFileLogService _fileLogService;
    private readonly ILogMergeService _mergeService;
    private readonly ILogger<LogSearchService> _logger;

    public LogSearchService(
        IEnvironmentService environmentService,
        IDatabaseLogService databaseLogService,
        IFileLogService fileLogService,
        ILogMergeService mergeService,
        ILogger<LogSearchService> logger)
    {
        _environmentService = environmentService;
        _databaseLogService = databaseLogService;
        _fileLogService = fileLogService;
        _mergeService = mergeService;
        _logger = logger;
    }

    public async Task<SearchLogsResponse> SearchLogsAsync(SearchLogsRequest request)
    {
        // Get environment configuration
        var config = _environmentService.GetConfiguration(request.Environment);

        // Set default date range if not provided (24 hours back)
        var fromDate = request.FromDate ?? DateTime.Now.AddHours(-24);
        var toDate = request.ToDate ?? DateTime.Now;

        var dbLogs = new List<LogEntry>();
        var fileLogs = new List<LogEntry>();

        // Retrieve logs from database (with error handling)
        try
        {
            dbLogs = await _databaseLogService.GetLogsAsync(
                request.CaseNumber, 
                fromDate, 
                toDate, 
                config.ConnectionString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve logs from database for case {CaseNumber}", 
                request.CaseNumber);
            // Continue with file logs
        }

        // Retrieve logs from files (with error handling)
        try
        {
            fileLogs = await _fileLogService.GetLogsAsync(
                request.CaseNumber, 
                fromDate, 
                toDate, 
                config.LogDirectory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve logs from files for case {CaseNumber}", 
                request.CaseNumber);
            // Continue with db logs
        }

        // Merge and sort logs
        var mergedLogs = _mergeService.MergeAndSort(dbLogs, fileLogs);

        return new SearchLogsResponse
        {
            Logs = mergedLogs
        };
    }
}
