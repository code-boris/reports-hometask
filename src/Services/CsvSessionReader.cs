using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Models;
using TaskConsoleApp.Resources;

namespace TaskConsoleApp.Services;

/// <inheritdoc />
public class CsvSessionReader(ITraceLogger logger) : ISessionReader
{
    /// <inheritdoc />
    public async Task<IEnumerable<Session>> ReadSessionsAsync(string? filePath)
    {
        var sessions = new List<Session>();

        try
        {
            if (filePath != null)
            {
                using var streamReader = new StreamReader(filePath);

                while (await streamReader.ReadLineAsync() is { } line)
                {
                    try
                    {
                        var session = Session.Create(line, logger);
                        if (session != null)
                        {
                            sessions.Add(session);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log(string.Format(ConfigConstants.ErrorParsingLineFailureMessage, line, ex));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Log(string.Format(ConfigConstants.ErrorReadingFileFailureMessage, filePath, ex));
        }

        return sessions;
    }
}