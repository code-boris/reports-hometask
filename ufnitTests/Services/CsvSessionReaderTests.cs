using Moq;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Resources;
using TaskConsoleApp.Services;

namespace UnitTests.Services;

public class CsvSessionReaderTests
{
    private readonly Mock<ITraceLogger> _mockLogger;
    private readonly CsvSessionReader _csvSessionReader;

    public CsvSessionReaderTests()
    {
        _mockLogger = new Mock<ITraceLogger>();
        _csvSessionReader = new CsvSessionReader(_mockLogger.Object);
    }

    [Fact]
    public async Task ReadSessionsAsync_ValidFile_ReturnsSessions()
    {
        // Arrange
        const string filePath = "validfile.csv";
        const string fileContent = "14.10.2020 15:35:41;14.10.2020 15:36:06;МАГРАМ NPS октябрь 2020;Шатохина Арина Александровна;Разговор;26\n" +
                                   "23.10.2020 09:04:35;23.10.2020 09:04:51;МАГРАМ Банки 2020;Парфенова Александра Александровна;Готов;16\n" +
                                   "30.10.2020 12:28:05;30.10.2020 12:29:24;МАГРАМ NPS октябрь 2020;Чечухина Марина Анатольевна;Разговор;78";

        await File.WriteAllTextAsync(filePath, fileContent);

        // Act
        var sessions = await _csvSessionReader.ReadSessionsAsync(filePath);

        // Assert
        Assert.Equal(3, sessions.Count());
        File.Delete(filePath);
    }

    [Fact]
    public async Task ReadSessionsAsync_InvalidLine_LogsError()
    {
        // Arrange
        const string filePath = "invalidfile.csv";
        const string fileContent = "Invalid Line";

        await File.WriteAllTextAsync(filePath, fileContent);

        // Act
        var sessions = await _csvSessionReader.ReadSessionsAsync(filePath);

        // Assert
        Assert.Empty(sessions);
        _mockLogger.Verify(logger => logger.Log(It.Is<string>(msg => msg.Contains(string.Format(ConfigConstants.ErrorParsingLineFailureMessage, fileContent, null)))), Times.Once);
        File.Delete(filePath); // Cleanup the created file
    }

    [Fact]
    public async Task ReadSessionsAsync_FileNotFound_LogsError()
    {
        // Arrange
        const string filePath = "nonexistentfile.csv";

        // Act
        var sessions = await _csvSessionReader.ReadSessionsAsync(filePath);

        // Assert
        Assert.Empty(sessions);
        _mockLogger.Verify(logger => logger.Log(It.Is<string>(msg => msg.Contains("Error reading file"))), Times.Once);
    }
}