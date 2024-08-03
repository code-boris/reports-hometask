namespace TaskConsoleApp.Interfaces;

/// <summary>
/// Сервис по сессиям
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Сгенерировать отчёт
    /// </summary>
    /// <param name="inputFilePath">Путь к файлу</param>
    /// <returns></returns>
    Task GenerateReportAsync(string? inputFilePath);
}