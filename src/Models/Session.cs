using System.Globalization;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Resources;

namespace TaskConsoleApp.Models;

/// <summary>
/// Сессия
/// </summary>
public class Session
{
    /// <summary>
    /// Дата начала сессии
    /// </summary>
    public DateTime Start { get; }
    
    /// <summary>
    /// Дата окончания сессии
    /// </summary>
    public DateTime End { get; }
    
    /// <summary>
    /// Название проекта
    /// </summary>
    public string Project { get; }
    
    /// <summary>
    /// Наименование оператора
    /// </summary>
    public string Operator { get; }
    
    /// <summary>
    /// Город
    /// </summary>
    public string State { get; }
    
    /// <summary>
    /// Длительность
    /// </summary>
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