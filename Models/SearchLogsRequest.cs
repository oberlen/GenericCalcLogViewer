using System.ComponentModel.DataAnnotations;

namespace GenericCalcLogViewer.Models;

/// <summary>
/// מודל בקשת חיפוש לוגים
/// </summary>
public class SearchLogsRequest
{
    /// <summary>
    /// סביבת ההרצה: DEV, TEST, INT, PROD
    /// </summary>
    [Required(ErrorMessage = "Environment is required")]
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// מספר התיק לחיפוש
    /// </summary>
    [Required(ErrorMessage = "CaseNumber is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CaseNumber must be greater than zero")]
    public int CaseNumber { get; set; }

    /// <summary>
    /// תאריך התחלה (אופציונלי, ברירת מחדל: 24 שעות אחורה)
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// תאריך סיום (אופציונלי, ברירת מחדל: זמן נוכחי)
    /// </summary>
    public DateTime? ToDate { get; set; }
}
