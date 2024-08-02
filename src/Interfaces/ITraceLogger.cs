namespace TaskConsoleApp.Interfaces;

public interface ITraceLogger
{
    void Log(string message);
    
    void Log(Func<string> message);
    
    void LogPaths(string message, Func<IEnumerable<string?>> paths);
}