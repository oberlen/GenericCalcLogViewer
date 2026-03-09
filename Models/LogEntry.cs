namespace GenericCalcLogViewer.Models;

/// <summary>
/// מייצג רשומת לוג אחת לאחר פרסור
/// </summary>
public class LogEntry
{
    /// <summary>
    /// זמן יצירת הלוג
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// מקור הלוג: "DB" או "FILE"
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// רמת הלוג: INFO, WARN, ERROR, DEBUG
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// תוכן הודעת הלוג
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// מספר התיק המזהה את ריצת החישוב
    /// </summary>
    public string CaseNumber { get; set; } = string.Empty;
}
