using Moq;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Services;

namespace UnitTests.Services;

public class ReportServiceTests
{
    private readonly IReportService _reportService;
    private readonly ISessionReader _csvSessionReader;

    public ReportServiceTests()
    {
        Mock<ITraceLogger> mockLogger = new();
        _reportService = new ReportService(mockLogger.Object);
        _csvSessionReader = new CsvSessionReader(mockLogger.Object);
    }

    [Fact]
    public async Task GenerateFirstReportAsync_GeneratesCorrectReport()
    {
        // Arrange
        const string filePath = "validfile.csv";
        const string fileContent = 
            "14.10.2020 15:35:41;14.10.2020 15:36:06;МАГРАМ NPS октябрь 2020;Шатохина Арина Александровна;Разговор;26\n" +
            "23.10.2020 09:04:35;23.10.2020 09:04:51;МАГРАМ Банки 2020;Парфенова Александра Александровна;Готов;16\n" +
            "30.10.2020 12:28:05;30.10.2020 12:29:24;МАГРАМ NPS октябрь 2020;Чечухина Марина Анатольевна;Разговор;78";

        await File.WriteAllTextAsync(filePath, fileContent);

        var sessions = await _csvSessionReader.ReadSessionsAsync(filePath);

        const string expectedOutput = "День       Количество сессий\r\n14.10.2020 1\r\n23.10.2020 1\r\n30.10.2020 1\r\n\r\n";

        await using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _reportService.GenerateFirstReportAsync(sessions);

        // Assert
        var actualOutput = stringWriter.ToString();
        Assert.Equal(expectedOutput, actualOutput);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public async Task GenerateSecondReportAsync_GeneratesCorrectReport()
    {
        // Arrange
        const string filePath = "validfile.csv";
        const string fileContent = 
            "14.10.2020 15:35:41;14.10.2020 15:36:06;МАГРАМ NPS октябрь 2020;Шатохина Арина Александровна;Разговор;26\n" +
            "23.10.2020 09:04:35;23.10.2020 09:04:51;МАГРАМ Банки 2020;Парфенова Александра Александровна;Готов;16\n" +
            "30.10.2020 12:28:05;30.10.2020 12:29:24;МАГРАМ NPS октябрь 2020;Чечухина Марина Анатольевна;Разговор;78";

        await File.WriteAllTextAsync(filePath, fileContent);

        var sessions = await _csvSessionReader.ReadSessionsAsync(filePath);

        const string expectedOutput = 
            "ФИО                                Пауза Готов Разговор Обработка Перезвон\r\n" +
            "Шатохина Арина Александровна           0     0       26         0        0\r\n" +
            "Парфенова Александра Александровна     0    16        0         0        0\r\n" +
            "Чечухина Марина Анатольевна            0     0       78         0        0\r\n\r\n";

        await using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _reportService.GenerateSecondReportAsync(sessions);

        // Assert
        var actualOutput = stringWriter.ToString();
        Assert.Equal(expectedOutput, actualOutput);

        // Cleanup
        File.Delete(filePath);
    }
}