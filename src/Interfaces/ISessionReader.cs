using TaskConsoleApp.Models;

namespace TaskConsoleApp.Interfaces;

public interface ISessionReader
{
    Task<IEnumerable<Session>> ReadSessionsAsync(string filePath);
}