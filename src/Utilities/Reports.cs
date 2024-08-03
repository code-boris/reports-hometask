using System.Globalization;
using TaskConsoleApp.Resources;

namespace TaskConsoleApp.Utilities;

public static class Reports
{
    /// <summary>
    /// Получить максимальную ширину для столбца
    /// </summary>
    /// <param name="operatorStates"></param>
    /// <returns></returns>
    public static (int nameWidth, int pauseWidth, int readyWidth, int talkWidth, int processingWidth, int recallWidth) 
        GetColumnWidths(Dictionary<string, Dictionary<string, TimeSpan>> operatorStates)
    {
        var nameWidth = Math.Max(operatorStates.Keys.Select(name => 
            name.Length).Max(), StringConstants.Fio.Length);
        var pauseWidth = Math.Max(operatorStates.Values.Select(states => 
                states[StringConstants.Pause].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), 
            StringConstants.Pause.Length);
        var readyWidth = Math.Max(operatorStates.Values.Select(states => 
                states[StringConstants.Ready].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), 
            StringConstants.Ready.Length);
        var talkWidth = Math.Max(operatorStates.Values.Select(states => 
                states[StringConstants.Talk].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), 
            StringConstants.Talk.Length);
        var processingWidth = Math.Max(operatorStates.Values.Select(states => 
                states[StringConstants.Processing].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), 
            StringConstants.Processing.Length);
        var recallWidth = Math.Max(operatorStates.Values.Select(states => 
                states[StringConstants.Recall].TotalSeconds.ToString(CultureInfo.InvariantCulture).Length).Max(), 
            StringConstants.Recall.Length);

        return (nameWidth, pauseWidth, readyWidth, talkWidth, processingWidth, recallWidth);
    }
    
    /// <summary>
    /// Напечатать первый отчёт
    /// </summary>
    /// <param name="report">Данные для отчёта</param>
    public static void PrintFirstReport(IEnumerable<dynamic> report)
    {
        Console.WriteLine(StringConstants.DateHeader);
        foreach (var entry in report)
        {
            Console.WriteLine(ConfigConstants.ReportService_PrintFirstReport__0_dd_MM_yyyy___1_, entry.Date, entry.MaxConcurrentSessions);
        }
        Console.WriteLine();
    }
    
    /// <summary>
    /// Напечатать хэдер для второго отчёта
    /// </summary>
    /// <param name="columnWidths">Ширины столбцов</param>
    public static void PrintSecondReportHeader((int nameWidth, int pauseWidth, int readyWidth, int talkWidth, int processingWidth, int recallWidth) columnWidths)
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
}