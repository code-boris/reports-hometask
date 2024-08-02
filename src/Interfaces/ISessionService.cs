namespace TaskConsoleApp.Interfaces;

public interface ISessionService
{
    Task GenerateReportAsync(string inputFilePath);
}