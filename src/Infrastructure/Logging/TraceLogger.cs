using TaskConsoleApp.Interfaces;

namespace TaskConsoleApp.Infrastructure.Logging;

/// <summary>
/// TraceLogger is used when the --trace flag is provided, and logs debugging details to a file.
/// </summary>
internal sealed class TraceLogger : ITraceLogger
{
    private readonly string _path;

    private TraceLogger(string path) => _path = path;

    public void Log(string message) => File.AppendAllText(_path, $"{DateTime.UtcNow:s} - {message}{Environment.NewLine}");

    public void Log(Func<string> message) => Log(message());

    public void LogPaths(string message, Func<IEnumerable<string?>> paths) => Log(message + ": " + GroupPathsByPrefixForLogging(paths()));

    public static ITraceLogger Create(string path)
    {
        var tracePath = Path.GetFullPath(path);
        var logger = new TraceLogger(tracePath);

        // let the user know where the trace is being logged to, by writing to the REPL.
        Console.Write(Environment.NewLine + ConfigConstants.WritingTraceToLogMessage);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(tracePath + Environment.NewLine);
        Console.ResetColor();

        AppDomain.CurrentDomain.UnhandledException +=
            (_, evt) => logger.Log(ConfigConstants.UnhandledExceptionMessage + evt.ExceptionObject);
        TaskScheduler.UnobservedTaskException +=
            (_, evt) => logger.Log(ConfigConstants.UnobervedTaskExceptionMessage + evt.Exception);

        logger.Log(ConfigConstants.TraceSessionStartingMessage);

        return logger;
    }

    private static string GroupPathsByPrefixForLogging(IEnumerable<string?> paths) =>
        string.Join(
            ", ",
            paths
                .GroupBy(Path.GetDirectoryName)
                .Select(group => $"""
                                  "{group.Key}": [{string.Join(", ", group.Select(path => 
                                     $"""
                                      "{Path.GetFileName(path)}"
                                      """))}]
                                  """)
        );
}