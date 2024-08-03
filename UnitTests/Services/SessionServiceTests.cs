using Bogus;
using Moq;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Models;
using TaskConsoleApp.Resources;
using TaskConsoleApp.Services;

namespace UnitTests.Services;

public class SessionServiceTests
{
    private readonly Mock<ISessionReader> _mockSessionReader;
    private readonly Mock<IReportService> _mockReportService;
    private readonly Mock<ITraceLogger> _mockLogger;
    private readonly SessionService _sessionService;
    private readonly Faker<Session> _sessionFaker;

    public SessionServiceTests()
    {
        _mockSessionReader = new Mock<ISessionReader>();
        _mockReportService = new Mock<IReportService>();
        _mockLogger = new Mock<ITraceLogger>();
        _sessionService = new SessionService(_mockSessionReader.Object, _mockReportService.Object, _mockLogger.Object);
        
        _sessionFaker = new Faker<Session>()
            .CustomInstantiator(f => new Session(
                f.Date.Past(), 
                f.Date.Recent(), 
                f.Commerce.ProductName(), 
                f.Person.FullName,
                f.Random.Word(),
                f.Date.Timespan(TimeSpan.FromHours(1))
            ));
    }

    [Fact]
    public async Task GenerateReportAsync_InputFileNotExists_LogsError()
    {
        // Arrange
        string inputFilePath = "nonexistentfile.txt";
        _mockLogger.Setup(logger => logger.Log(It.IsAny<string>()));

        // Act
        await _sessionService.GenerateReportAsync(inputFilePath);

        // Assert
        _mockLogger.Verify(logger => logger.Log(string.Format(ConfigConstants.FileNotFoundFailureMessage, inputFilePath)), Times.Once);
    }

    [Fact]
    public async Task GenerateReportAsync_ValidInputFile_GeneratesReports()
    {
        // Arrange
        string inputFilePath = "existingfile.txt";
        File.Create(inputFilePath).Dispose(); // Create the file to simulate existence
        
        var sessions = _sessionFaker.Generate(10); // Generate a list of 10 random sessions


        _mockSessionReader.Setup(reader => reader.ReadSessionsAsync(inputFilePath))
            .ReturnsAsync(sessions);
        _mockReportService.Setup(service => service.GenerateFirstReportAsync(It.IsAny<IEnumerable<Session>>()))
            .Returns(Task.CompletedTask);
        _mockReportService.Setup(service => service.GenerateSecondReportAsync(It.IsAny<IEnumerable<Session>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sessionService.GenerateReportAsync(inputFilePath);

        // Assert
        _mockSessionReader.Verify(reader => reader.ReadSessionsAsync(inputFilePath), Times.Once);
        _mockReportService.Verify(service => service.GenerateFirstReportAsync(It.Is<IEnumerable<Session>>(e => e.SequenceEqual(sessions))), Times.Once);
        _mockReportService.Verify(service => service.GenerateSecondReportAsync(It.Is<IEnumerable<Session>>(e => e.SequenceEqual(sessions))), Times.Once);

        // Cleanup
        File.Delete(inputFilePath); // Cleanup the created file
    }
}