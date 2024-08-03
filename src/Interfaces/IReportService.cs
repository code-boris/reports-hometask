using TaskConsoleApp.Models;

namespace TaskConsoleApp.Interfaces;

/// <summary>
/// Сервис для составления отчётов
/// </summary>
/// <param name="logger">Экземпляр логгера</param>
public interface IReportService
{
    /// <summary>
    /// Сгенерировать первый отчёт
    /// </summary>
    /// <param name="sessions">Коллекция сеансов</param>
    Task GenerateFirstReportAsync(IEnumerable<Session> sessions);
    
    /// <summary>
    /// Сгенерировать второй отчёт
    /// </summary>
    /// <param name="sessions">Данные по сессиям</param>
    Task GenerateSecondReportAsync(IEnumerable<Session> sessions);
}