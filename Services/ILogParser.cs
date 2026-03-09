using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// ממשק לשירות פרסור שורות לוג
/// </summary>
public interface ILogParser
{
    /// <summary>
    /// מפרסר שורת לוג גולמית לאובייקט LogEntry
    /// </summary>
    /// <param name="logLine">שורת לוג גולמית</param>
    /// <param name="caseNumber">מספר תיק</param>
    /// <returns>אובייקט LogEntry או null אם הפרסור נכשל</returns>
    LogEntry? Parse(string logLine, string caseNumber);

    /// <summary>
    /// מפרמט אובייקט LogEntry חזרה למחרוזת לוג
    /// </summary>
    /// <param name="logEntry">אובייקט LogEntry</param>
    /// <returns>מחרוזת לוג מפורמטת</returns>
    string Format(LogEntry logEntry);
}
