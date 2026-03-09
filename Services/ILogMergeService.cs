using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// ממשק לשירות מיזוג ומיון לוגים
/// </summary>
public interface ILogMergeService
{
    /// <summary>
    /// ממזג וממיין לוגים ממקורות שונים
    /// </summary>
    /// <param name="dbLogs">לוגים ממסד נתונים</param>
    /// <param name="fileLogs">לוגים מקבצים</param>
    /// <returns>רשימה מאוחדת וממוינת של לוגים</returns>
    List<LogEntry> MergeAndSort(List<LogEntry> dbLogs, List<LogEntry> fileLogs);
}
