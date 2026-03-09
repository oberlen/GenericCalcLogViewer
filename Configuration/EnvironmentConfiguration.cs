namespace GenericCalcLogViewer.Configuration;

/// <summary>
/// תצורה המכילה Database connection string ו-LogDirectory path לכל סביבה
/// </summary>
public class EnvironmentConfiguration
{
    /// <summary>
    /// שם הסביבה
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// מחרוזת חיבור למסד הנתונים
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// נתיב תיקיית קבצי הלוג
    /// </summary>
    public string LogDirectory { get; set; } = string.Empty;
}
