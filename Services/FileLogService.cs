using System.Globalization;
using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// שירות אחזור לוגים מקבצי runtime באמצעות streaming
/// </summary>
public class FileLogService : IFileLogService
{
    private readonly ILogParser _parser;
    private readonly ILogger<FileLogService> _logger;

    public FileLogService(ILogParser parser, ILogger<FileLogService> logger)
    {
        _parser = parser;
        _logger = logger;
    }

    public async Task<List<LogEntry>> GetLogsAsync(
        int caseNumber, 
        DateTime fromDate, 
        DateTime toDate, 
        string logDirectory)
    {
        var logs = new List<LogEntry>();

        if (!Directory.Exists(logDirectory))
        {
            throw new DirectoryNotFoundException($"Log directory not found: {logDirectory}");
        }

        // Identify relevant log files based on date range
        var relevantFiles = GetRelevantLogFiles(logDirectory, fromDate, toDate);

        _logger.LogInformation("Found {Count} relevant log files for case {CaseNumber}", 
            relevantFiles.Count, caseNumber);

        foreach (var filePath in relevantFiles)
        {
            try
            {
                var fileLogs = await ReadLogsFromFileAsync(filePath, caseNumber);
                logs.AddRange(fileLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read log file: {FilePath}", filePath);
                // Continue with next file
            }
        }

        // Filter by date range
        logs = logs.Where(log => log.Timestamp >= fromDate && log.Timestamp <= toDate).ToList();

        _logger.LogInformation("Retrieved {Count} logs from files for case {CaseNumber}", 
            logs.Count, caseNumber);

        return logs;
    }

    private List<string> GetRelevantLogFiles(string logDirectory, DateTime fromDate, DateTime toDate)
    {
        var allFiles = Directory.GetFiles(logDirectory, "*.log", SearchOption.TopDirectoryOnly);
        
        // Assuming log files are named with date pattern: calc_YYYYMMDD.log
        var relevantFiles = allFiles
            .Where(file =>
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.Length >= 8)
                {
                    var datePart = fileName.Substring(fileName.Length - 8);
                    if (DateTime.TryParseExact(datePart, "yyyyMMdd", null, 
                        DateTimeStyles.None, out var fileDate))
                    {
                        return fileDate.Date >= fromDate.Date && fileDate.Date <= toDate.Date;
                    }
                }
                return true; // Include files without date pattern
            })
            .OrderBy(f => f)
            .ToList();

        return relevantFiles;
    }

    private async Task<List<LogEntry>> ReadLogsFromFileAsync(string filePath, int caseNumber)
    {
        var logs = new List<LogEntry>();
        var caseNumberStr = caseNumber.ToString();

        // Streaming read - line by line
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fileStream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            // Filter lines containing case number
            if (line.Contains(caseNumberStr))
            {
                try
                {
                    var logEntry = _parser.Parse(line, caseNumberStr);
                    if (logEntry != null)
                    {
                        logEntry.Source = "FILE";
                        logs.Add(logEntry);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse log line: {Line}", line);
                    // Continue with next line
                }
            }
        }

        return logs;
    }
}
