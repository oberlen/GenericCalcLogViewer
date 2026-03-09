using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// ממשק לשירות אחזור לוגים מקבצים
/// </summary>
public interface IFileLogService
{
    /// <summary>
    /// מאחזר לוגים מקבצי runtime באמצעות streaming
    /// </summary>
    /// <param name="caseNumber">מספר תיק</param>
    /// <param name="fromDate">תאריך התחלה</param>
    /// <param name="toDate">תאריך סיום</param>
    /// <param name="logDirectory">נתיב תיקיית לוגים</param>
    /// <returns>רשימת לוגים</returns>
    Task<List<LogEntry>> GetLogsAsync(int caseNumber, DateTime fromDate, DateTime toDate, string logDirectory);
}
