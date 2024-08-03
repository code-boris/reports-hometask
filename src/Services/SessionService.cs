using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Models;
using TaskConsoleApp.Resources;

namespace TaskConsoleApp.Services;

/// <inheritdoc />
public class SessionService(ISessionReader sessionReader, IReportService reportService, ITraceLogger logger) : ISessionService
{
    /// <inheritdoc />
    public async Task GenerateReportAsync(string? inputFilePath)
    {
        if (!File.Exists(inputFilePath))
        {
            logger.Log(string.Format(ConfigConstants.FileNotFoundFailureMessage, inputFilePath));
            return;
        }

        var sessions = await sessionReader.ReadSessionsAsync(inputFilePath);
        var enumerable = sessions as Session[] ?? sessions.ToArray();
        await reportService.GenerateFirstReportAsync(enumerable);
        await reportService.GenerateSecondReportAsync(enumerable);
    }
}