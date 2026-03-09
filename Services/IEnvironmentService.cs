using GenericCalcLogViewer.Configuration;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// ממשק לשירות ניהול תצורת סביבה
/// </summary>
public interface IEnvironmentService
{
    /// <summary>
    /// מחזיר תצורה לסביבה מבוקשת
    /// </summary>
    /// <param name="environment">שם הסביבה (DEV, TEST, INT, PROD)</param>
    /// <returns>אובייקט EnvironmentConfiguration</returns>
    EnvironmentConfiguration GetConfiguration(string environment);
}
