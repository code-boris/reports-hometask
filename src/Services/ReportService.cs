using System.Globalization;
using System.Text.RegularExpressions;
using TaskConsoleApp.Infrastructure;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Models;

namespace TaskConsoleApp.Services;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public partial class ReportService(ITraceLogger logger)
{
    private static readonly Regex FullNameRegex = MyRegex();
    
    public async Task GenerateFirstReportAsync(IEnumerable<Session> sessions)
    {
        var dailySessions = GetDailySessions(sessions);

        var report = dailySessions.Select(d => new
        {
            Date = d.Key,
            MaxConcurrentSessions = GetMaxConcurrentSessions(d.Value).Result
        });

        PrintFirstReport(report);
    }

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

    private static void PrintFirstReport(IEnumerable<dynamic> report)
    {
        Console.WriteLine(StringConstants.DateHeader);
        foreach (var entry in report)
        {
            Console.WriteLine($"{entry.Date:dd.MM.yyyy} {entry.MaxConcurrentSessions}");
        }
        Console.WriteLine();
    }

    public async Task GenerateSecondReportAsync(IEnumerable<Session> sessions)
    {
        var operatorStates = GetOperatorStates(sessions);

        var columnWidths = GetColumnWidths(operatorStates);

        PrintSecondReportHeader(columnWidths);

        PrintSecondReport(operatorStates, columnWidths);
    }

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

    private (int nameWidth, int pauseWidth, int readyWidth, int talkWidth, int processingWidth, int recallWidth) 
        GetColumnWidths(Dictionary<string, Dictionary<string, TimeSpan>> operatorStates)
    {
        var nameWidth = Math.Max(operatorStates.Keys.Select(name => name.Length).Max(), StringConstants.Fio.Length);
        var pauseWidth = Math.Max(operatorStates.Values.Select(states => 
            states[StringConstants.Pause].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), StringConstants.Pause.Length);
        var readyWidth = Math.Max(operatorStates.Values.Select(states => 
            states[StringConstants.Ready].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), StringConstants.Ready.Length);
        var talkWidth = Math.Max(operatorStates.Values.Select(states => 
            states[StringConstants.Talk].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), StringConstants.Talk.Length);
        var processingWidth = Math.Max(operatorStates.Values.Select(states => 
            states[StringConstants.Processing].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), StringConstants.Processing.Length);
        var recallWidth = Math.Max(operatorStates.Values.Select(states => 
            states[StringConstants.Recall].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), StringConstants.Recall.Length);

        return (nameWidth, pauseWidth, readyWidth, talkWidth, processingWidth, recallWidth);
    }

    private static void PrintSecondReportHeader((int nameWidth, int pauseWidth, int readyWidth, int talkWidth, int processingWidth, int recallWidth) columnWidths)
    {
        Console.WriteLine(
            "{0,-" + columnWidths.nameWidth + "} " +
            "{1," + columnWidths.pauseWidth + "} " +
            "{2," + columnWidths.readyWidth + "} " +
            "{3," + columnWidths.talkWidth + "} " +
            "{4," + columnWidths.processingWidth + "} " +
            "{5," + columnWidths.recallWidth + "}",
            StringConstants.Fio,
            StringConstants.Pause,
            StringConstants.Ready,
            StringConstants.Talk,
            StringConstants.Processing,
            StringConstants.Recall);
    }

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
