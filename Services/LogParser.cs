using System.Text.RegularExpressions;
using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// שירות פרסור שורות לוג גולמיות לאובייקטים מובנים
/// </summary>
public class LogParser : ILogParser
{
    private readonly ILogger<LogParser> _logger;
    
    // Regex pattern: YYYY-MM-DD HH:MM:SS LEVEL Message
    private static readonly Regex LogPattern = new Regex(
        @"^(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})\s+(\w+)\s+(.+)$",
        RegexOptions.Compiled);

    public LogParser(ILogger<LogParser> logger)
    {
        _logger = logger;
    }

    public LogEntry? Parse(string logLine, string caseNumber)
    {
        if (string.IsNullOrWhiteSpace(logLine))
        {
            return null;
        }

        var match = LogPattern.Match(logLine);
        if (!match.Success)
        {
            _logger.LogWarning("Log line does not match expected format: {Line}", logLine);
            return null;
        }

        try
        {
            var timestamp = DateTime.Parse(match.Groups[1].Value);
            var level = match.Groups[2].Value;
            var message = match.Groups[3].Value;

            return new LogEntry
            {
                Timestamp = timestamp,
                Level = level,
                Message = message,
                CaseNumber = caseNumber,
                Source = string.Empty // Will be set by caller
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse log line: {Line}", logLine);
            return null;
        }
    }

    public string Format(LogEntry logEntry)
    {
        if (logEntry == null)
        {
            throw new ArgumentNullException(nameof(logEntry));
        }

        return $"{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss} {logEntry.Level} {logEntry.Message}";
    }
}
