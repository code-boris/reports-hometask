using TaskConsoleApp.Interfaces;

namespace TaskConsoleApp.Infrastructure.Logging;

/// <summary>
/// NullLogger is used by default. <see cref="TraceLogger"/> is used when the --trace flag is provided.
/// </summary>
internal sealed class NullLogger : ITraceLogger
{
    /// <inheritdoc />
    public void Log(string message) { /* null logger does not log */ }
    
    /// <inheritdoc />
    public void Log(Func<string> message) { /* null logger does not log */ }
    
    /// <inheritdoc />
    public void LogPaths(string message, Func<IEnumerable<string?>> paths) { /* null logger does not log */ }
}