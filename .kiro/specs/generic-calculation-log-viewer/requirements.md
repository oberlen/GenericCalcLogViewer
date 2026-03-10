# מסמך דרישות - Generic Calculation Log Viewer

## מבוא

המערכת היא כלי פנימי לדיבאג ומוניטורינג עבור שירות Generic Calculation. המטרה היא לאפשר למפתחים ולמהנדסי תמיכה לאתר ולנתח לוגים שנוצרו במהלך ריצות חישוב. המערכת מאחזרת לוגים משני מקורות (מסד נתונים SQL Server וקבצי לוג runtime), ממזגת אותם ומציגה אותם בציר זמן כרונולוגי מאוחד.

## מילון מונחים

- **Log_Viewer**: המערכת המרכזית המספקת ממשק חיפוש ותצוגה של לוגים
- **Environment_Service**: רכיב האחראי על טעינת תצורת סביבה
- **Database_Log_Service**: רכיב המבצע שאילתות SQL לאחזור לוגים ממסד נתונים
- **File_Log_Service**: רכיב הקורא קבצי לוג מתיקיית הקבצים
- **Log_Parser**: רכיב המפרסר שורות לוג גולמיות לאובייקטים מובנים
- **Log_Merge_Service**: רכיב המשלב וממיין לוגים ממקורות שונים
- **Log_Search_Service**: רכיב המתאם את פעולת החיפוש המלאה
- **Logs_Controller**: בקר Web API המטפל בבקשות HTTP לחיפוש לוגים
- **Case_Number**: מספר תיק ייחודי המזהה ריצת חישוב
- **Environment**: סביבת הרצה (DEV, TEST, INT, PROD)
- **Log_Entry**: אובייקט לוג בודד המכיל Timestamp, Source, Level, Message, CaseNumber
- **SearchLogsRequest**: אובייקט בקשה המכיל Environment, CaseNumber, FromDate, ToDate
- **SearchLogsResponse**: אובייקט תגובה המכיל רשימת Logs
- **Supported_Environment**: אחת מארבע הסביבות הנתמכות (DEV, TEST, INT, PROD)
- **Environment_Configuration**: תצורה המכילה Database connection string ו-LogDirectory path לכל סביבה
- **Streaming_Read**: קריאת קבצים שורה אחר שורה ללא טעינת כל הקובץ לזיכרון
- **DB_Index**: אינדקס מסד נתונים על (ms_merkaz_req, op_time) לשיפור ביצועי שאילתות
- **IEnvironmentService**: ממשק לשירות ניהול תצורת סביבה
- **IDatabaseLogService**: ממשק לשירות אחזור לוגים ממסד נתונים
- **IFileLogService**: ממשק לשירות אחזור לוגים מקבצים
- **ILogParser**: ממשק לשירות פרסור שורות לוג
- **ILogMergeService**: ממשק לשירות מיזוג ומיון לוגים
- **ILogSearchService**: ממשק לשירות תיאום חיפוש לוגים
- **Dependency_Injection_Container**: מנגנון .NET לניהול מחזור חיים של שירותים והזרקת תלויות
- **Service_Lifetime**: מדיניות מחזור חיים של שירות (Singleton, Scoped, Transient)
- **Validation**: תהליך אימות תקינות נתוני בקשה לפני ביצוע פעולה
- **Calculation_Run**: רצף לוגים המייצג ריצת חישוב אחת, מתחילה ב-"Calculation started" ומסתיימת ב-"Calculation finished"

## מודלי נתונים

### מודל LogEntry
- **Timestamp**: DateTime - זמן יצירת הלוג
- **Source**: string - מקור הלוג (DB או FILE)
- **Level**: string - רמת הלוג (INFO, WARN, ERROR, DEBUG)
- **Message**: string - תוכן הודעת הלוג
- **CaseNumber**: string - מספר התיק המזהה את ריצת החישוב

### מודל SearchLogsRequest
- **Environment**: string - סביבת ההרצה (DEV, TEST, INT, PROD)
- **CaseNumber**: int - מספר התיק לחיפוש
- **FromDate**: DateTime? - תאריך התחלה (אופציונלי)
- **ToDate**: DateTime? - תאריך סיום (אופציונלי)

### מודל SearchLogsResponse
- **Logs**: List<LogEntry> - רשימת הלוגים שנמצאו

## מבנה הפרויקט

המערכת תפותח כ-.NET Web API עם המבנה הבא:

```
GenericCalcLogViewer
├── Controllers
│   └── LogsController
├── Services
│   ├── EnvironmentService
│   ├── DatabaseLogService
│   ├── FileLogService
│   ├── LogParser
│   ├── LogMergeService
│   └── LogSearchService
├── Models
│   ├── LogEntry
│   ├── SearchLogsRequest
│   └── SearchLogsResponse
└── Configuration
    └── EnvironmentConfiguration
```

## תצורת סביבות

כל סביבה מוגדרת עם מסד נתונים ותיקיית לוגים:

- **DEV**: Database: Gvia_DEV, LogDirectory: \\dev-server\logs\calc
- **TEST**: Database: Gvia_TEST, LogDirectory: \\test-server\logs\calc
- **INT**: Database: Gvia_INT, LogDirectory: \\int-server\logs\calc
- **PROD**: Database: Gvia_PROD, LogDirectory: \\prod-server\logs\calc

## פורמט קבצי לוג

קבצי הלוג בפורמט:
```
YYYY-MM-DD HH:MM:SS LEVEL Message
```

דוגמה:
```
2026-03-09 10:01:03 INFO Calculation started for case 19193496
```

## דרישות

### דרישה 1: חיפוש לוגים לפי מספר תיק וסביבה

**User Story:** כמפתח או מהנדס תמיכה, אני רוצה לחפש לוגים לפי מספר תיק וסביבה, כדי לאתר ולנתח בעיות בריצות חישוב ספציפיות.

#### קריטריוני קבלה

1. WHEN המשתמש מזין Case_Number ו-Environment ולוחץ על כפתור החיפוש, THE Log_Search_Service SHALL אחזר לוגים מכל המקורות הרלוונטיים
2. THE Log_Viewer SHALL דרוש מהמשתמש להזין Case_Number ו-Environment כשדות חובה
3. WHERE המשתמש לא מספק טווח תאריכים, THE Log_Search_Service SHALL אחזר לוגים מ-24 השעות האחרונות
4. THE Log_Viewer SHALL תמוך בסביבות DEV, TEST, INT, ו-PROD בלבד

### דרישה 2: סינון לוגים לפי טווח תאריכים

**User Story:** כמפתח, אני רוצה לסנן לוגים לפי טווח תאריכים, כדי להתמקד בתקופת זמן ספציפית.

#### קריטריוני קבלה

1. WHERE המשתמש מספק From_DateTime ו-To_DateTime, THE Log_Search_Service SHALL אחזר לוגים בטווח התאריכים שצוין בלבד
2. WHEN המשתמש מספק From_DateTime בלבד, THE Log_Search_Service SHALL אחזר לוגים מהתאריך שצוין ועד לזמן הנוכחי
3. WHEN המשתמש מספק To_DateTime בלבד, THE Log_Search_Service SHALL אחזר לוגים עד לתאריך שצוין מהתחלת היום

### דרישה 3: אחזור לוגים ממסד נתונים

**User Story:** כמפתח, אני רוצה לאחזר לוגים ממסד הנתונים, כדי לקבל גישה ללוגים מובנים שנשמרו על ידי השירות.

#### קריטריוני קבלה

1. WHEN המשתמש מבצע חיפוש, THE Database_Log_Service SHALL בצע שאילתה לטבלה gvia.srv_GenericCalc_log
2. THE Database_Log_Service SHALL השתמש ב-NOLOCK hint בשאילתת SQL
3. THE Database_Log_Service SHALL סנן לוגים לפי ms_merkaz_req השווה ל-Case_Number שצוין
4. THE Database_Log_Service SHALL סנן לוגים לפי op_time בטווח התאריכים שצוין
5. THE Database_Log_Service SHALL מיין תוצאות לפי op_time בסדר עולה
6. THE Database_Log_Service SHALL מפה שדות מסד נתונים (ms_merkaz_req, op_time, log_level, message) לאובייקטי Log_Entry
7. THE Database_Log_Service SHALL הסתמך על DB_Index (ms_merkaz_req, op_time) לביצועים אופטימליים
8. IF חיבור למסד הנתונים נכשל, THEN THE Database_Log_Service SHALL החזר שגיאה תיאורית

### דרישה 4: אחזור לוגים מקבצים

**User Story:** כמפתח, אני רוצה לאחזר לוגים מקבצי runtime, כדי לקבל גישה ללוגים שנכתבו ישירות לקבצים על ידי השירות.

#### קריטריוני קבלה

1. WHEN המשתמש מבצע חיפוש, THE File_Log_Service SHALL זהה קבצי לוג התואמים לטווח התאריכים המבוקש
2. THE File_Log_Service SHALL פתח כל קובץ לוג רלוונטי
3. THE File_Log_Service SHALL קרא שורות מקבצי הלוג שורה אחר שורה באמצעות Streaming_Read
4. THE File_Log_Service SHALL לא טען קובץ לוג שלם לזיכרון בו זמנית
5. THE File_Log_Service SHALL סנן שורות המכילות את Case_Number שצוין
6. THE File_Log_Service SHALL העבר שורות מסוננות ל-Log_Parser לפרסור
7. IF תיקיית לוג לא נגישה, THEN THE File_Log_Service SHALL החזר שגיאה תיאורית
8. IF קריאת קובץ נכשלת, THEN THE File_Log_Service SHALL רשום את השגיאה והמשך לקובץ הבא

### דרישה 5: פרסור שורות לוג

**User Story:** כמפתח, אני רוצה שהמערכת תפרסר שורות לוג גולמיות, כדי להציג אותן בפורמט מובנה.

#### קריטריוני קבלה

1. WHEN שורת לוג גולמית מועברת ל-Log_Parser, THE Log_Parser SHALL חלץ timestamp, level, ו-message
2. THE Log_Parser SHALL תמוך בפורמט לוג: "YYYY-MM-DD HH:MM:SS LEVEL Message"
3. THE Log_Parser SHALL המיר timestamp למבנה DateTime
4. THE Log_Parser SHALL יצור אובייקט Log_Entry עם השדות המפורסרים
5. THE Pretty_Printer SHALL פרמט אובייקטי Log_Entry חזרה למחרוזות לוג תקינות
6. FOR ALL אובייקטי Log_Entry תקינים, פרסור ואז הדפסה ואז פרסור SHALL יצור אובייקט Log_Entry שווה ערך (round-trip property)

### דרישה 6: מיזוג ומיון לוגים

**User Story:** כמפתח, אני רוצה לראות לוגים ממקורות שונים בציר זמן מאוחד, כדי להבין את רצף האירועים המלא.

#### קריטריוני קבלה

1. WHEN לוגים מאוחזרים ממקורות מרובים, THE Log_Merge_Service SHALL שלב את כל הלוגים לרשימה אחת
2. THE Log_Merge_Service SHALL מיין לוגים לפי Timestamp בסדר כרונולוגי עולה
3. WHEN שני לוגים בעלי Timestamp זהה, THE Log_Merge_Service SHALL מיקם לוגים עם Source=FILE לפני לוגים עם Source=DB
4. THE Log_Merge_Service SHALL שמור את שדה Source בכל Log_Entry לזיהוי מקור הלוג
5. THE Log_Merge_Service SHALL החזר רשימה מאוחדת וממוינת של אובייקטי Log_Entry

### דרישה 7: תצוגת תוצאות חיפוש

**User Story:** כמפתח, אני רוצה לראות תוצאות חיפוש בטבלה ברורה, כדי לנתח לוגים בקלות.

#### קריטריוני קבלה

1. WHEN תוצאות חיפוש מתקבלות, THE Log_Viewer SHALL הצג אותן בטבלה
2. THE Log_Viewer SHALL הצג עמודות: Timestamp, Source, Level, Message
3. THE Log_Viewer SHALL הצג לוגים בסדר כרונולוגי עולה
4. WHERE מספר התוצאות עולה על 500 רשומות, THE Log_Viewer SHALL החל pagination

### דרישה 8: ניהול תצורת סביבה

**User Story:** כמפתח, אני רוצה שהמערכת תטען תצורה ספציפית לכל סביבה, כדי להתחבר למקורות הנתונים הנכונים.

#### קריטריוני קבלה

1. WHEN סביבה נבחרת, THE Environment_Service SHALL טען מחרוזת חיבור למסד נתונים ספציפית לסביבה
2. WHEN סביבה נבחרת, THE Environment_Service SHALL טען נתיב תיקיית קבצי לוג ספציפי לסביבה
3. THE Environment_Service SHALL תמוך בתצורות עבור DEV, TEST, INT, ו-PROD
4. THE Environment_Service SHALL טען תצורה מאובייקט Environment_Configuration
5. IF סביבה לא נתמכת מסופקת, THEN THE Environment_Service SHALL החזר שגיאה תיאורית

### דרישה 9: API לחיפוש לוגים

**User Story:** כמפתח frontend, אני רוצה API מוגדר היטב לחיפוש לוגים, כדי לשלב את הממשק עם ה-backend.

#### קריטריוני קבלה

1. THE Logs_Controller SHALL חשוף endpoint POST ב-/api/logs/search
2. WHEN בקשה מתקבלת, THE Logs_Controller SHALL קבל אובייקט SearchLogsRequest עם שדות: Environment, CaseNumber, FromDate (אופציונלי), ToDate (אופציונלי)
3. WHEN חיפוש מושלם, THE Logs_Controller SHALL החזר אובייקט SearchLogsResponse עם מערך Logs המכיל אובייקטי LogEntry
4. IF בקשה חסרה שדות חובה, THEN THE Logs_Controller SHALL החזר קוד שגיאה 400 עם הודעה תיאורית
5. IF שגיאה פנימית מתרחשת, THEN THE Logs_Controller SHALL החזר קוד שגיאה 500 עם הודעה תיאורית

### דרישה 10: אבטחה והרשאות

**User Story:** כמנהל מערכת, אני רוצה להגביל גישה למערכת, כדי להבטיח שרק משתמשים מורשים יוכלו לצפות בלוגים.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL תהיה נגישה מרשת פנימית בלבד
2. WHERE מערכת אימות פנימית קיימת, THE Log_Viewer SHALL השתמש במערכת האימות הפנימית
3. WHEN משתמש לא מאומת מנסה לגשת למערכת, THE Log_Viewer SHALL דחה את הבקשה עם קוד שגיאה 401

### דרישה 11: טיפול בשגיאות ולוגינג

**User Story:** כמפתח, אני רוצה שהמערכת תטפל בשגיאות בצורה חסינה, כדי להבטיח שהמערכת תמשיך לפעול גם במקרה של כשלים חלקיים.

#### קריטריוני קבלה

1. IF Database_Log_Service נכשל, THEN THE Log_Search_Service SHALL רשום את השגיאה והמשך עם File_Log_Service
2. IF File_Log_Service נכשל, THEN THE Log_Search_Service SHALL רשום את השגיאה והמשך עם Database_Log_Service
3. WHEN שגיאה מתרחשת, THE Log_Viewer SHALL רשום את השגיאה עם פרטים מלאים
4. WHEN שגיאה מתרחשת, THE Log_Viewer SHALL החזר הודעת שגיאה תיאורית למשתמש
5. THE Log_Viewer SHALL כלול בהודעת שגיאה את סוג השגיאה ופעולה מומלצת למשתמש

### דרישה 12: ביצועי שאילתות מסד נתונים

**User Story:** כמפתח, אני רוצה שחיפושי לוגים יהיו מהירים, כדי לספק חוויית משתמש טובה.

#### קריטריוני קבלה

1. THE Database_Log_Service SHALL השתמש ב-DB_Index על (ms_merkaz_req, op_time) לביצוע שאילתות
2. WHEN חיפוש טיפוסי מבוצע, THE Log_Search_Service SHALL החזר תוצאות תוך 2 שניות
3. THE Log_Search_Service SHALL תמוך באחזור עד 10,000 רשומות לוג לכל חיפוש
4. THE Database_Log_Service SHALL מגביל תוצאות שאילתה ל-10,000 רשומות מקסימום

### דרישה 13: ממשק משתמש לחיפוש

**User Story:** כמשתמש, אני רוצה ממשק חיפוש אינטואיטיבי, כדי לבצע חיפושי לוגים בקלות.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL הצג שדה קלט טקסט עבור Case_Number
2. THE Log_Viewer SHALL הצג רשימה נפתחת עבור Environment עם אפשרויות DEV, TEST, INT, PROD
3. THE Log_Viewer SHALL הצג בורר תאריך ושעה עבור From_DateTime
4. THE Log_Viewer SHALL הצג בורר תאריך ושעה עבור To_DateTime
5. THE Log_Viewer SHALL הצג כפתור Search להפעלת החיפוש
6. WHEN המשתמש לוחץ על כפתור Search, THE Log_Viewer SHALL שלח בקשת SearchLogsRequest ל-API

### דרישה 14: תצוגת טבלת תוצאות

**User Story:** כמשתמש, אני רוצה לראות תוצאות בטבלה מסודרת, כדי לנתח לוגים בצורה יעילה.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL הצג תוצאות בטבלה עם עמודות: Timestamp, Source, Level, Message
2. THE Log_Viewer SHALL מיין תוצאות לפי Timestamp בסדר עולה כברירת מחדל
3. WHERE מספר התוצאות עולה על 500 רשומות, THE Log_Viewer SHALL החל pagination
4. THE Log_Viewer SHALL הצג מספר עמוד נוכחי וסך כל העמודים

### דרישה 15: שיפורים עתידיים

**User Story:** כמנהל מוצר, אני רוצה לתכנן שיפורים עתידיים, כדי לשפר את המערכת בהדרגה.

#### קריטריוני קבלה

1. THE Log_Viewer MAY תמוך בויזואליזציה של ציר זמן חישוב בגרסאות עתידיות
2. THE Log_Viewer MAY תמוך בהתראות אוטומטיות על שגיאות בגרסאות עתידיות

### דרישה 16: הגדרת Interfaces לשירותים

**User Story:** כמפתח, אני רוצה שכל השירותים יוגדרו עם Interfaces, כדי לאפשר Dependency Injection ותחזוקה קלה של הקוד.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL הגדיר ממשקים לכל השירותים
2. THE Log_Viewer SHALL כלול את הממשקים הבאים: IEnvironmentService, IDatabaseLogService, IFileLogService, ILogParser, ILogMergeService, ILogSearchService
3. THE ILogSearchService SHALL חשוף מתודה: SearchLogs(SearchLogsRequest request)
4. THE Environment_Service SHALL מימש את IEnvironmentService
5. THE Database_Log_Service SHALL מימש את IDatabaseLogService
6. THE File_Log_Service SHALL מימש את IFileLogService
7. THE Log_Parser SHALL מימש את ILogParser
8. THE Log_Merge_Service SHALL מימש את ILogMergeService
9. THE Log_Search_Service SHALL מימש את ILogSearchService

### דרישה 17: Dependency Injection

**User Story:** כמפתח, אני רוצה שכל השירותים יהיו רשומים במנגנון Dependency Injection של .NET, כדי לאפשר ארכיטקטורה מודולרית וניהול מחזור חיים של שירותים.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL רשום את כל השירותים ב-Dependency_Injection_Container
2. THE Environment_Service SHALL רשום עם Service_Lifetime של Singleton
3. THE Database_Log_Service SHALL רשום עם Service_Lifetime של Scoped
4. THE File_Log_Service SHALL רשום עם Service_Lifetime של Scoped
5. THE Log_Parser SHALL רשום עם Service_Lifetime של Singleton
6. THE Log_Merge_Service SHALL רשום עם Service_Lifetime של Singleton
7. THE Log_Search_Service SHALL רשום עם Service_Lifetime של Scoped
8. THE Log_Viewer SHALL בצע רישום שירותים במהלך אתחול האפליקציה

### דרישה 18: אימות נתוני בקשה (Validation)

**User Story:** כמפתח, אני רוצה לוודא שנתוני הבקשה תקינים לפני ביצוע החיפוש, כדי למנוע שגיאות מיותרות.

#### קריטריוני קבלה

1. THE Logs_Controller SHALL אמת ש-CaseNumber גדול מאפס
2. THE Logs_Controller SHALL אמת ש-Environment הוא אחד מהערכים: DEV, TEST, INT, PROD
3. IF FromDate ו-ToDate קיימים שניהם, THEN THE Logs_Controller SHALL אמת ש-FromDate מוקדם מ-ToDate
4. IF Validation נכשל, THEN THE Logs_Controller SHALL החזר קוד HTTP 400 עם הודעת שגיאה תיאורית

### דרישה 19: סיכום תוצאות החיפוש

**User Story:** כמשתמש, אני רוצה לראות כמה לוגים נמצאו, כדי להבין את היקף התוצאות.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL הצג את מספר הלוגים הכולל שהוחזרו
2. THE Log_Viewer SHALL הצג את המספר מעל טבלת התוצאות
3. THE Log_Viewer SHALL הצג את המספר בפורמט: "Logs found: [מספר]"

### דרישה 20: סימון שגיאות בלוגים

**User Story:** כמפתח, אני רוצה לראות לוגים עם שגיאות בצורה מודגשת, כדי לזהות בעיות במהירות.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL הדגיש לוגים על בסיס שדה Level
2. WHERE Level שווה ל-ERROR, THE Log_Viewer SHALL הצג את שורת הלוג בצבע אדום
3. WHERE Level שווה ל-WARN, THE Log_Viewer SHALL הצג את שורת הלוג בצבע כתום
4. WHERE Level שווה ל-INFO, THE Log_Viewer SHALL הצג את שורת הלוג בצורה רגילה

### דרישה 21: סינון לוגים בתוך התוצאות

**User Story:** כמשתמש, אני רוצה לחפש מילה בתוך הלוגים שהוחזרו, כדי לאתר אירועים ספציפיים במהירות.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL ספק שדה טקסט לסינון מעל טבלת התוצאות
2. WHEN המשתמש מזין טקסט בשדה הסינון, THE Log_Viewer SHALL סנן לוגים המכילים את הטקסט בשדה Message
3. THE Log_Viewer SHALL בצע סינון ללא תלות באותיות גדולות/קטנות (case-insensitive)

### דרישה 22: ייצוא לוגים

**User Story:** כמפתח, אני רוצה לייצא את תוצאות החיפוש לקובץ, כדי לשתף לוגים עם צוותים אחרים.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL אפשר ייצוא לוגים לפורמט CSV
2. THE Log_Viewer SHALL כלול בקובץ המיוצא את העמודות הבאות: Timestamp, Source, Level, Message
3. THE Log_Viewer SHALL כלול בקובץ המיוצא את כל הלוגים המוצגים כעת בטבלה

### דרישה 23: זיהוי ריצות חישוב (Calculation Run Detection)

**User Story:** כמפתח, אני רוצה לראות לוגים מקובצים לפי ריצות חישוב, כדי להבין את רצף האירועים של כל ריצה.

#### קריטריוני קבלה

1. THE Log_Viewer SHALL נסה לזהות Calculation_Run בתוך ציר הזמן של הלוגים
2. WHEN הודעת לוג מכילה "Calculation started", THE Log_Viewer SHALL סמן התחלת Calculation_Run
3. WHEN הודעת לוג מכילה "Calculation finished", THE Log_Viewer SHALL סמן סיום Calculation_Run
4. IF סמני התחלה או סיום לא נמצאים, THEN THE Log_Viewer SHALL הפריד ריצות על בסיס פערי זמן גדולים מ-2 דקות
5. THE Log_Viewer SHALL אפשר קיבוץ לוגים לפי Calculation_Run שזוהו בגרסאות עתידיות של הממשק
