namespace TaskConsoleApp.Infrastructure;

public class AppConfig(string inputFilePath)
{
    public string InputFilePath { get; init; } = inputFilePath;
    public bool EnableLogging { get; init; }
}
    
public class LoggingConfig(string logFileNameFormat)
{
    public string LogFileNameFormat { get; init; } = logFileNameFormat;
}