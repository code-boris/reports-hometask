using System.Text.RegularExpressions;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Models;
using TaskConsoleApp.Resources;
using TaskConsoleApp.Utilities;

namespace TaskConsoleApp.Services;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
/// <summary>
/// Сервис для составления отчётов
/// </summary>
/// <param name="logger">Экземпляр логгера</param>
public partial class ReportService(ITraceLogger logger)
{
    /// <summary>
    /// Регулярное выражение для проверки на полное имя
    /// </summary>
    private static readonly Regex FullNameRegex = MyRegex();
    
    /// <summary>
    /// Сгенерировать первый отчёт
    /// </summary>
    /// <param name="sessions">Коллекция сеансов</param>
    public async Task GenerateFirstReportAsync(IEnumerable<Session> sessions)
    {
        var dailySessions = GetDailySessions(sessions);

        var report = dailySessions.Select(d => new
        {
            Date = d.Key,
            MaxConcurrentSessions = GetMaxConcurrentSessions(d.Value).Result
        });

        Reports.PrintFirstReport(report);
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
    /// Сгенерировать второй отчёт
    /// </summary>
    /// <param name="sessions">Данные по сессиям</param>
    public async Task GenerateSecondReportAsync(IEnumerable<Session> sessions)
    {
        var operatorStates = GetOperatorStates(sessions);

        var columnWidths = Reports.GetColumnWidths(operatorStates);

        Reports.PrintSecondReportHeader(columnWidths);

        PrintSecondReport(operatorStates, columnWidths);
    }

    /// <summary>
    /// Обработать данные по сессиям (коллекции), чтобы сагрерировать общее время потраченное каждым оператором в различных состояниях (states).
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
    /// Напечатать второй отчёт
    /// </summary>
    /// <param name="operatorStates">Состояния операторов</param>
    /// <param name="columnWidths">Ширины столбцов</param>
    private static void PrintSecondReport(Dictionary<string, Dictionary<string, TimeSpan>> operatorStates, 
        (int nameWidth, int pauseWidth, int readyWidth, int talkWidth, int processingWidth, int recallWidth) columnWidths)
    {
        foreach (var (operatorName, states) in operatorStates)
        {
            if (!FullNameRegex.IsMatch(operatorName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            
            Console.WriteLine(
                "{0,-" + columnWidths.nameWidth + "} " +
                "{1," + columnWidths.pauseWidth + "} " +
                "{2," + columnWidths.readyWidth + "} " +
                "{3," + columnWidths.talkWidth + "} " +
                "{4," + columnWidths.processingWidth + "} " +
                "{5," + columnWidths.recallWidth + "}",
                operatorName,
                states[StringConstants.Pause].TotalSeconds,
                states[StringConstants.Ready].TotalSeconds,
                states[StringConstants.Talk].TotalSeconds,
                states[StringConstants.Processing].TotalSeconds,
                states[StringConstants.Recall].TotalSeconds);
            
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    private async ValueTask<int> GetMaxConcurrentSessions(IEnumerable<(DateTime Start, DateTime End)> sessions)
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

    [GeneratedRegex(@"^[А-ЯЁ][а-яё]+\s[А-ЯЁ][а-яё]+\s[А-ЯЁ][а-яё]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
