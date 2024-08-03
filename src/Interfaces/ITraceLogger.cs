namespace TaskConsoleApp.Interfaces;

/// <summary>
/// Логгер
/// </summary>
public interface ITraceLogger
{
    /// <summary>
    /// Логировать по переданному сообщению
    /// </summary>
    /// <param name="message">Сообщение</param>
    void Log(string message);
    
    /// <summary>
    /// Логировать и применить функцию внутри
    /// </summary>
    /// <param name="message">Сообщение</param>
    void Log(Func<string> message);
    
    /// <summary>
    /// Логирование по путям к файлам
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="paths">Пути к файлам</param>
    /// <returns></returns>
    void LogPaths(string message, Func<IEnumerable<string?>> paths);
}