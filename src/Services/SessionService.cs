using TaskConsoleApp.Infrastructure;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Models;

namespace TaskConsoleApp.Services;

public class SessionService(ISessionReader sessionReader, ReportService reportService, ITraceLogger logger) : ISessionService
{
    public async Task GenerateReportAsync(string inputFilePath)
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