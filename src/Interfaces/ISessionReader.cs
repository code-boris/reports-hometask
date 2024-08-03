using TaskConsoleApp.Models;

namespace TaskConsoleApp.Interfaces;

/// <summary>
/// Чтение CSV файла
/// </summary>
public interface ISessionReader
{
    /// <summary>
    /// Метод для чтения сессий из CSV файла
    /// </summary>
    /// <param name="filePath">Путь до файла</param>
    /// <returns>Коллекция сессии</returns>
    Task<IEnumerable<Session>> ReadSessionsAsync(string? filePath);
}