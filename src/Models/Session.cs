using System.Globalization;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Resources;

namespace TaskConsoleApp.Models;

/// <summary>
/// Сессия
/// </summary>
public class Session(DateTime start, DateTime end, string project, string @operator, string state, TimeSpan duration)
{
    /// <summary>
    /// Дата начала сессии
    /// </summary>
    public DateTime Start { get; } = start;

    /// <summary>
    /// Дата окончания сессии
    /// </summary>
    public DateTime End { get; } = end;

    /// <summary>
    /// Название проекта
    /// </summary>
    public string Project { get; } = project;

    /// <summary>
    /// Наименование оператора
    /// </summary>
    public string Operator { get; } = @operator;

    /// <summary>
    /// Город
    /// </summary>
    public string State { get; } = state;

    /// <summary>
    /// Длительность
    /// </summary>
    public TimeSpan Duration { get; } = duration;

    /// <summary>
    /// Создать объект сессии на основе CSV файла
    /// </summary>
    /// <param name="csvLine"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
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