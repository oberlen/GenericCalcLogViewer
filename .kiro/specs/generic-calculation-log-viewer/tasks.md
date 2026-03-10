# תוכנית יישום - Generic Calculation Log Viewer

## סקירה כללית

מסמך זה מגדיר את המשימות ליישום מערכת Generic Calculation Log Viewer - כלי פנימי לדיבאג ומוניטורינג המאפשר למפתחים ולמהנדסי תמיכה לאתר ולנתח לוגים שנוצרו במהלך ריצות חישוב. המערכת מאחזרת לוגים משני מקורות (SQL Server וקבצי לוג), ממזגת אותם ומציגה בציר זמן כרונולוגי מאוחד.

**טכנולוגיות**: .NET 6+ Web API, C#, SQL Server, ADO.NET

## משימות

- [x] 1. הקמת פרויקט ומבנה בסיסי
  - צור פרויקט .NET Web API חדש בשם GenericCalcLogViewer
  - צור מבנה תיקיות: Controllers, Services, Models, Configuration
  - הוסף חבילות NuGet נדרשות: System.Data.SqlClient, Microsoft.Extensions.Logging
  - הגדר appsettings.json עם מחרוזות חיבור ונתיבי לוג לכל סביבה (DEV, TEST, INT, PROD)
  - _דרישות: 8.1, 8.2, 8.3_

- [x] 2. יצירת מודלי נתונים
  - [x] 2.1 צור מחלקת LogEntry
    - שדות: Timestamp (DateTime), Source (string), Level (string), Message (string), CaseNumber (string)
    - הוסף XML documentation לכל שדה
    - _דרישות: 5.4_
  
  - [x] 2.2 צור מחלקת SearchLogsRequest
    - שדות: Environment (string, Required), CaseNumber (int, Required, Range(1, int.MaxValue)), FromDate (DateTime?), ToDate (DateTime?)
    - הוסף Data Annotations לאימות
    - _דרישות: 1.2, 18.1, 18.2_
  
  - [x] 2.3 צור מחלקת SearchLogsResponse
    - שדה: Logs (List<LogEntry>)
    - תכונה מחושבת: TotalCount
    - _דרישות: 9.3_
  
  - [x] 2.4 צור מחלקת EnvironmentConfiguration
    - שדות: Environment (string), ConnectionString (string), LogDirectory (string)
    - _דרישות: 8.4_

- [x] 3. יישום שירות ניהול תצורת סביבה
  - [x] 3.1 צור ממשק IEnvironmentService
    - מתודה: GetConfiguration(string environment) → EnvironmentConfiguration
    - _דרישות: 16.2_
  
  - [x] 3.2 יישם מחלקת EnvironmentService
    - טען תצורות מ-IConfiguration בבנאי
    - אתחל Dictionary עם תצורות לכל 4 הסביבות
    - מימוש GetConfiguration עם אימות סביבה תקינה
    - זרוק ArgumentException לסביבה לא נתמכת
    - _דרישות: 8.1, 8.2, 8.3, 8.5_
  
  - [ ]* 3.3 כתוב property test לשירות תצורת סביבה
    - **Property 15: Environment Configuration Retrieval**
    - **מאמת: דרישות 8.1, 8.2, 8.3**

- [x] 4. יישום שירות פרסור לוגים
  - [x] 4.1 צור ממשק ILogParser
    - מתודות: Parse(string logLine, string caseNumber) → LogEntry, Format(LogEntry logEntry) → string
    - _דרישות: 16.6_
  
  - [x] 4.2 יישם מחלקת LogParser
    - הגדר Regex pattern: `^(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})\s+(\w+)\s+(.+)$`
    - מימוש Parse: התאמת regex, חילוץ קבוצות, יצירת LogEntry
    - מימוש Format: פורמט חזרה למחרוזת לוג
    - טיפול בשגיאות: החזר null לשורות לא תקינות, רשום אזהרה
    - _דרישות: 5.1, 5.2, 5.3, 5.4, 5.5_
  
  - [ ]* 4.3 כתוב property test לפרסור לוגים
    - **Property 6: Log Parsing**
    - **מאמת: דרישות 5.1, 5.2, 5.3, 5.4**
  
  - [ ]* 4.4 כתוב property test לפורמט לוגים
    - **Property 7: Log Formatting**
    - **מאמת: דרישות 5.5**
  
  - [ ]* 4.5 כתוב property test ל-round-trip
    - **Property 8: Round-Trip Parsing**
    - **מאמת: דרישות 5.6**

- [x] 5. יישום שירות מיזוג ומיון לוגים
  - [x] 5.1 צור ממשק ILogMergeService
    - מתודה: MergeAndSort(List<LogEntry> dbLogs, List<LogEntry> fileLogs) → List<LogEntry>
    - _דרישות: 16.8_
  
  - [x] 5.2 יישם מחלקת LogMergeService
    - צור רשימה מאוחדת מכל המקורות
    - מיין לפי Timestamp עולה, ואז לפי Source (FILE לפני DB)
    - רשום מידע על מספר הלוגים שאוחדו
    - _דרישות: 6.1, 6.2, 6.3, 6.5_
  
  - [ ]* 5.3 כתוב property test למיזוג ומיון
    - **Property 9: Log Merging and Sorting**
    - **מאמת: דרישות 6.1, 6.2, 6.3, 6.5**
  
  - [ ]* 5.4 כתוב unit test לבדיקת סדר עם timestamp זהה
    - בדוק ש-FILE מגיע לפני DB כאשר Timestamp זהה
    - _דרישות: 6.3_

- [ ] 6. Checkpoint - וידוא מבנה בסיסי
  - ודא שכל המודלים והשירותים הבסיסיים עוברים קומפילציה
  - הרץ את כל הבדיקות שנכתבו עד כה
  - שאל את המשתמש אם יש שאלות או בעיות

- [x] 7. יישום שירות אחזור לוגים ממסד נתונים
  - [x] 7.1 צור ממשק IDatabaseLogService
    - מתודה: GetLogsAsync(int caseNumber, DateTime fromDate, DateTime toDate, string connectionString) → Task<List<LogEntry>>
    - _דרישות: 16.3_
  
  - [x] 7.2 יישם מחלקת DatabaseLogService
    - בנה שאילתת SQL עם TOP 10000, NOLOCK, WHERE על ms_merkaz_req ו-op_time, ORDER BY op_time
    - השתמש ב-SqlConnection ו-SqlCommand עם פרמטרים
    - מפה שדות DB (ms_merkaz_req, op_time, log_level, message) ל-LogEntry
    - הגדר Source = "DB"
    - טיפול ב-SqlException: רשום שגיאה וזרוק InvalidOperationException
    - _דרישות: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 12.4_
  
  - [ ]* 7.3 כתוב property test למיפוי שדות DB
    - **Property 12: Database Field Mapping**
    - **מאמת: דרישות 3.6**
  
  - [ ]* 7.4 כתוב property test לסינון טווח תאריכים
    - **Property 2: Date Range Filtering**
    - **מאמת: דרישות 2.1, 3.3, 3.4**
  
  - [ ]* 7.5 כתוב unit test לטיפול בכשל חיבור
    - בדוק שזריקת SqlException מטופלת נכון
    - _דרישות: 3.8_

- [x] 8. יישום שירות אחזור לוגים מקבצים
  - [x] 8.1 צור ממשק IFileLogService
    - מתודה: GetLogsAsync(int caseNumber, DateTime fromDate, DateTime toDate, string logDirectory) → Task<List<LogEntry>>
    - _דרישות: 16.4_
  
  - [x] 8.2 יישם מחלקת FileLogService
    - מימוש GetRelevantLogFiles: זיהוי קבצים לפי תבנית שם ותאריך
    - מימוש ReadLogsFromFileAsync: קריאת streaming עם FileStream ו-StreamReader
    - השתמש ב-FileShare.ReadWrite לקבצים פעילים
    - סנן שורות המכילות את CaseNumber
    - העבר שורות ל-ILogParser.Parse
    - הגדר Source = "FILE"
    - טיפול בשגיאות: רשום שגיאה והמשך לקובץ הבא
    - סנן לוגים לפי טווח תאריכים לאחר הקריאה
    - _דרישות: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8_
  
  - [ ]* 8.3 כתוב property test לסינון לוגים מקבצים
    - **Property 13: File Log Filtering**
    - **מאמת: דרישות 4.5**
  
  - [ ]* 8.4 כתוב property test לבחירת קבצים לפי תאריך
    - **Property 14: File Selection by Date Range**
    - **מאמת: דרישות 4.1**
  
  - [ ]* 8.5 כתוב unit test לטיפול בתיקייה לא נגישה
    - בדוק שזריקת DirectoryNotFoundException מטופלת נכון
    - _דרישות: 4.7_

- [x] 9. יישום שירות תיאום חיפוש
  - [x] 9.1 צור ממשק ILogSearchService
    - מתודה: SearchLogsAsync(SearchLogsRequest request) → Task<SearchLogsResponse>
    - _דרישות: 16.9_
  
  - [x] 9.2 יישם מחלקת LogSearchService
    - קבל תלויות: IEnvironmentService, IDatabaseLogService, IFileLogService, ILogMergeService, ILogger
    - מימוש SearchLogsAsync:
      - קבל תצורת סביבה מ-IEnvironmentService
      - הגדר ברירת מחדל לטווח תאריכים (24 שעות אחורה אם לא צוין)
      - קרא לוגים מ-DB (עם try-catch, המשך בכשל)
      - קרא לוגים מקבצים (עם try-catch, המשך בכשל)
      - מזג ומיין לוגים עם ILogMergeService
      - החזר SearchLogsResponse
    - _דרישות: 1.1, 1.3, 11.1, 11.2_
  
  - [ ]* 9.3 כתוב property test לטווח תאריכים ברירת מחדל
    - **Property 3: Default Date Range**
    - **מאמת: דרישות 1.3**
  
  - [ ]* 9.4 כתוב property test לטווח תאריכים חלקי - From בלבד
    - **Property 4: Partial Date Range - From Only**
    - **מאמת: דרישות 2.2**
  
  - [ ]* 9.5 כתוב property test לטווח תאריכים חלקי - To בלבד
    - **Property 5: Partial Date Range - To Only**
    - **מאמת: דרישות 2.3**
  
  - [ ]* 9.6 כתוב unit test לטיפול בכשל חלקי
    - בדוק שכשל ב-DB לא מונע אחזור מקבצים
    - בדוק שכשל בקבצים לא מונע אחזור מ-DB
    - _דרישות: 11.1, 11.2_

- [ ] 10. Checkpoint - וידוא שירותים מרכזיים
  - ודא שכל השירותים עוברים קומפילציה ועובדים יחד
  - הרץ את כל הבדיקות שנכתבו עד כה
  - שאל את המשתמש אם יש שאלות או בעיות

- [x] 11. יישום Controller ו-API
  - [x] 11.1 צור מחלקת LogsController
    - הוסף [ApiController] ו-[Route("api/logs")]
    - קבל תלויות: ILogSearchService, ILogger<LogsController>
    - _דרישות: 9.1_
  
  - [x] 11.2 יישם endpoint POST /api/logs/search
    - מתודה: Search([FromBody] SearchLogsRequest request) → Task<ActionResult<SearchLogsResponse>>
    - מימוש ValidateRequest: בדוק CaseNumber > 0, Environment תקין, FromDate < ToDate
    - החזר BadRequest (400) לבקשה לא תקינה עם הודעת שגיאה
    - קרא ל-ILogSearchService.SearchLogsAsync
    - החזר Ok (200) עם SearchLogsResponse
    - טיפול בשגיאות: רשום שגיאה והחזר StatusCode 500
    - _דרישות: 9.2, 9.3, 9.4, 9.5, 18.1, 18.2, 18.3, 18.4_
  
  - [ ]* 11.3 כתוב property test לאימות בקשות
    - **Property 1: Request Validation**
    - **מאמת: דרישות 1.2, 1.4, 18.1, 18.2, 18.3, 18.4, 9.4**
  
  - [ ]* 11.4 כתוב integration test ל-endpoint
    - בדוק בקשה תקינה מחזירה 200
    - בדוק בקשה לא תקינה מחזירה 400
    - _דרישות: 9.2, 9.4_

- [x] 12. הגדרת Dependency Injection
  - [x] 12.1 הגדר רישום שירותים ב-Program.cs
    - רשום IEnvironmentService → EnvironmentService (Singleton)
    - רשום ILogParser → LogParser (Singleton)
    - רשום ILogMergeService → LogMergeService (Singleton)
    - רשום IDatabaseLogService → DatabaseLogService (Scoped)
    - רשום IFileLogService → FileLogService (Scoped)
    - רשום ILogSearchService → LogSearchService (Scoped)
    - _דרישות: 17.1, 17.2, 17.3, 17.4, 17.5, 17.6, 17.7, 17.8_
  
  - [x] 12.2 הגדר logging
    - הוסף Console ו-Debug logging providers
    - הגדר MinimumLevel ל-Information
    - _דרישות: 11.3_
  
  - [x] 12.3 הגדר CORS (אופציונלי)
    - הוסף policy "AllowInternalNetwork" עם origins פנימיים
    - _דרישות: 10.1_

- [x] 13. יצירת קובץ תצורה מלא
  - [x] 13.1 עדכן appsettings.json
    - הוסף ConnectionStrings לכל 4 הסביבות (DEV, TEST, INT, PROD)
    - הוסף LogDirectories לכל 4 הסביבות
    - הגדר Logging levels
    - _דרישות: 8.1, 8.2, 8.3_
  
  - [x] 13.2 צור appsettings.Development.json
    - הגדרות ספציפיות לסביבת פיתוח
    - רמת logging מפורטת יותר

- [ ] 14. Checkpoint - וידוא אינטגרציה מלאה
  - ודא שהאפליקציה מתחילה בהצלחה
  - בדוק שכל השירותים נרשמו ב-DI container
  - הרץ את כל הבדיקות (unit + property + integration)
  - שאל את המשתמש אם יש שאלות או בעיות

- [ ]* 15. יישום תכונות מתקדמות (אופציונלי)
  - [ ]* 15.1 יישום זיהוי ריצות חישוב
    - צור מחלקת CalculationRun
    - יישם לוגיקת זיהוי לפי "Calculation started" ו-"Calculation finished"
    - יישם הפרדה לפי פערי זמן > 2 דקות
    - _דרישות: 23.1, 23.2, 23.3, 23.4_
  
  - [ ]* 15.2 כתוב property tests לזיהוי ריצות
    - **Property 16: Calculation Run Start Detection**
    - **Property 17: Calculation Run End Detection**
    - **Property 18: Calculation Run Separation by Time Gap**
    - **מאמת: דרישות 23.2, 23.3, 23.4**

- [ ]* 16. תיעוד ו-README
  - [ ]* 16.1 צור README.md
    - תיאור המערכת
    - הוראות התקנה והרצה
    - דוגמאות שימוש ב-API
    - מבנה הפרויקט
  
  - [ ]* 16.2 הוסף XML documentation
    - תעד את כל ה-public APIs
    - הוסף דוגמאות שימוש בהערות

- [ ]* 17. אופטימיזציות וביצועים
  - [ ]* 17.1 הוסף אינדקס למסד נתונים
    - צור סקריפט SQL ליצירת אינדקס על (ms_merkaz_req, op_time)
    - _דרישות: 3.7, 12.1_
  
  - [ ]* 17.2 הוסף Rate Limiting
    - הגדר middleware ל-rate limiting (60 requests/minute)
    - _דרישות: 10.1_

- [ ] 18. Checkpoint סופי
  - הרץ את כל הבדיקות ווודא שהכל עובר
  - בדוק code coverage (יעד: 80%+)
  - בצע smoke test ידני של ה-API
  - ודא שכל הדרישות המרכזיות מכוסות
  - שאל את המשתמש אם יש שאלות או צורך בשיפורים נוספים

## הערות

- משימות המסומנות ב-`*` הן אופציונליות וניתן לדלג עליהן ל-MVP מהיר
- כל משימה מפנה לדרישות הספציפיות שהיא מכסה לצורך מעקב
- Checkpoints מאפשרים אימות הדרגתי של התקדמות
- Property tests מאמתים תכונות אוניברסליות על פני קלטים רבים (100+ איטרציות)
- Unit tests מאמתים דוגמאות ספציפיות ומקרי קצה
