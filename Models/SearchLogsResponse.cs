namespace GenericCalcLogViewer.Models;

/// <summary>
/// מודל תגובת חיפוש לוגים
/// </summary>
public class SearchLogsResponse
{
    /// <summary>
    /// רשימת הלוגים שנמצאו
    /// </summary>
    public List<LogEntry> Logs { get; set; } = new List<LogEntry>();

    /// <summary>
    /// מספר הלוגים הכולל
    /// </summary>
    public int TotalCount => Logs?.Count ?? 0;
}
