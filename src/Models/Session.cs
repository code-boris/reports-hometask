using System.Globalization;
using TaskConsoleApp.Infrastructure;
using TaskConsoleApp.Interfaces;

namespace TaskConsoleApp.Models;

public class Session
{
    public DateTime Start { get; }
    public DateTime End { get; }
    public string Project { get; }
    public string Operator { get; }
    public string State { get; }
    public TimeSpan Duration { get; }

    private Session(DateTime start, DateTime end, string project, string @operator, string state, TimeSpan duration)
    {
        Start = start;
        End = end;
        Project = project;
        Operator = @operator;
        State = state;
        Duration = duration;
    }

    public static Session? Create(string csvLine, ITraceLogger logger)
    {
        try
        {
            var parts = csvLine.Split(';');
            var start = DateTime.ParseExact(parts[0], ConfigConstants.DateTimeFormat, CultureInfo.InvariantCulture);
            var end = DateTime.ParseExact(parts[1], ConfigConstants.DateTimeFormat, CultureInfo.InvariantCulture);
            var project = parts[2];
            var @operator = parts[3];
            var state = parts[4];
            var duration = TimeSpan.FromSeconds(int.Parse(parts[5]));

            return new Session(start, end, project, @operator, state, duration);
        }
        catch (Exception e)
        {
            logger.Log(string.Format(ConfigConstants.FailedToParseLineWarningMessage, csvLine, e.Message));
            return null;
        }
    }
}