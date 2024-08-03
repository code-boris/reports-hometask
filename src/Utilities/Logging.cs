using TaskConsoleApp.Infrastructure.Logging;
using TaskConsoleApp.Interfaces;

namespace TaskConsoleApp.Utilities;

public static class Logging
{
    public static ITraceLogger InitializeLogging(bool trace, string logFileNameFormat) =>
        trace ? TraceLogger.Create(string.Format(logFileNameFormat, DateTime.UtcNow)) : new NullLogger();
}