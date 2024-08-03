using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Models;
using TaskConsoleApp.Resources;
using TaskConsoleApp.Utilities;

namespace TaskConsoleApp.Services;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

/// <inheritdoc />
public class ReportService(ITraceLogger logger) : IReportService
{
    /// <inheritdoc />
    public async Task GenerateFirstReportAsync(IEnumerable<Session> sessions)
    {
        var dailySessions = GetDailySessions(sessions);

        var report = dailySessions.Select(d => new
        {
            Date = d.Key,
            MaxConcurrentSessions = GetMaxConcurrentSessions(d.Value)
        });

        Reports.PrintFirstReport(report);
    }
    
    /// <inheritdoc />
    public async Task GenerateSecondReportAsync(IEnumerable<Session> sessions)
    {
        var operatorStates = GetOperatorStates(sessions);

        var columnWidths = Reports.GetColumnWidths(operatorStates);

        Reports.PrintSecondReportHeader(columnWidths);

        Reports.PrintSecondReport(operatorStates, columnWidths);
    }

    /// <summary>
    /// Группировка сеансов (сессий) по дням, которые они охватывают, с учетом сеансов, которые могут охватывать несколько дней.
    /// </summary>
    /// <param name="sessions">Коллекция сеансов</param>
    /// <returns>Словарь, где ключ - дата, а значение - лист кортежей с датами (начало и конец)</returns>
    private static Dictionary<DateTime, List<(DateTime Start, DateTime End)>> GetDailySessions(IEnumerable<Session> sessions)
    {
        var dailySessions = new Dictionary<DateTime, List<(DateTime Start, DateTime End)>>();

        foreach (var session in sessions)
        {
            var start = session.Start;
            var end = session.End;
            var current = start.Date;

            while (current <= end.Date)
            {
                if (!dailySessions.ContainsKey(current)) dailySessions[current] = new List<(DateTime Start, DateTime End)>();

                dailySessions[current].Add((start, end));
                current = current.AddDays(1);
            }
        }

        return dailySessions;
    }

    /// <summary>
    /// Обработать данные по сеансам (коллекции), чтобы сагрерировать общее время потраченное каждым оператором в различных состояниях (states).
    /// </summary>
    /// <param name="sessions">Сессии</param>
    /// <returns>Словарь, где ключ - имена операторов, а значения - внутренний словарь (ключ - состояния, а значения - кумулятивная длительность)</returns>
    private Dictionary<string, Dictionary<string, TimeSpan>> GetOperatorStates(IEnumerable<Session> sessions)
    {
        var operatorStates = new Dictionary<string, Dictionary<string, TimeSpan>>();

        foreach (var session in sessions)
        {
            if (!operatorStates.ContainsKey(session.Operator))
            {
                operatorStates[session.Operator] = InitializeOperatorStates();
            }

            if (operatorStates[session.Operator].ContainsKey(session.State))
            {
                operatorStates[session.Operator][session.State] += session.Duration;
            }
            else
            {
                logger.Log(string.Format(StringConstants.UnknownStateWarning, session.State, session.Operator));
            }
        }

        return operatorStates;
    }

    /// <summary>
    /// Инициализировать состояния (states) операторов
    /// </summary>
    /// <returns>Словарь с ключами как имена, а значения - нулевая длительность</returns>
    private static Dictionary<string, TimeSpan> InitializeOperatorStates()
    {
        return new Dictionary<string, TimeSpan>
        {
            { StringConstants.Pause, TimeSpan.Zero },
            { StringConstants.Ready, TimeSpan.Zero },
            { StringConstants.Talk, TimeSpan.Zero },
            { StringConstants.Processing, TimeSpan.Zero },
            { StringConstants.Recall, TimeSpan.Zero }
        };
    }

    /// <summary>
    /// Вычислить максимальное количество одновременно активных сессий на основе списка временных интервалов 
    /// </summary>
    /// <param name="sessions">Перечисление кортежей, где каждый кортеж содержит дату и время начала (Start) и конца (End) сессии</param>
    /// <returns>Максимальное количество сессий, которые были активны одновременно в какой-то момент времени</returns>
    private static int GetMaxConcurrentSessions(IEnumerable<(DateTime Start, DateTime End)> sessions)
    {
        var events = sessions
            .SelectMany(session => new[] { (session.Start, 1), (session.End, -1) })
            .OrderBy(e => e)
            .ThenBy(e => e.Item2 == 1 ? 0 : 1)
            .ToList();

        var maxConcurrent = 0;
        var currentConcurrent = 0;

        foreach (var (_, type) in events)
        {
            currentConcurrent += type;
            if (currentConcurrent > maxConcurrent) maxConcurrent = currentConcurrent;
        }

        return maxConcurrent;
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
