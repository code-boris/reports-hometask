namespace TaskConsoleApp.Infrastructure
{
    public static class ConfigConstants
    {
        public const string AppSettingsFileName = "appsettings.json";
        public const string AppConfigSection = "AppConfig";
        public const string LogConfigSection = "Logging";
        public const string ConfigAppLoadFailureMessage = "Failed to load application configuration from appsettings.json";
        public const string ConfigLogLoadFailureMessage = "Failed to load logging configuration from appsettings.json";
        public const string NoInputFileFailureMessage = "Error: No input file path provided.";
        public const string FileNotFoundFailureMessage = "File not found: '{0}'";
        public const string ErrorParsingLineFailureMessage = "Error parsing line '{0}': '{1}'";
        public const string ErrorReadingFileFailureMessage =  "Error reading file '{0}': '{1}'";
        public const string ErrorOccuredFailureMessage =  "An error occurred.";
        public const string DateTimeFormat =  "dd.MM.yyyy HH:mm:ss";
        public const string FailedToParseLineWarningMessage =  "Warning: Failed to parse line '{0}'. Error: '{1}'";
        public const string WritingTraceToLogMessage =  "Writing trace log to ";
        public const string UnhandledExceptionMessage =  "Unhandled Exception: ";
        public const string UnobervedTaskExceptionMessage =  "Unhandled Exception: ";
        public const string TraceSessionStartingMessage =  "Trace session starting";
    }
}