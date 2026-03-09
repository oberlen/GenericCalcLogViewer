using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// ממשק לשירות אחזור לוגים ממסד נתונים
/// </summary>
public interface IDatabaseLogService
{
    /// <summary>
    /// מאחזר לוגים ממסד נתונים SQL Server
    /// </summary>
    /// <param name="caseNumber">מספר תיק</param>
    /// <param name="fromDate">תאריך התחלה</param>
    /// <param name="toDate">תאריך סיום</param>
    /// <param name="connectionString">מחרוזת חיבור למסד נתונים</param>
    /// <returns>רשימת לוגים</returns>
    Task<List<LogEntry>> GetLogsAsync(int caseNumber, DateTime fromDate, DateTime toDate, string connectionString);
}
