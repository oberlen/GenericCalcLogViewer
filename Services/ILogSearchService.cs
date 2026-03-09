using GenericCalcLogViewer.Models;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// ממשק לשירות תיאום חיפוש לוגים
/// </summary>
public interface ILogSearchService
{
    /// <summary>
    /// מבצע חיפוש לוגים מכל המקורות
    /// </summary>
    /// <param name="request">בקשת חיפוש</param>
    /// <returns>תגובת חיפוש עם לוגים</returns>
    Task<SearchLogsResponse> SearchLogsAsync(SearchLogsRequest request);
}
