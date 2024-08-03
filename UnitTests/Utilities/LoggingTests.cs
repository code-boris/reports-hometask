using TaskConsoleApp.Infrastructure.Logging;
using TaskConsoleApp.Utilities;

namespace UnitTests.Utilities;

public class LoggingTests
{
    [Fact]
    public void InitializeLogging_ShouldReturnNullLogger_WhenTraceIsFalse()
    {
        // Arrange
        var trace = false;
        var logFileNameFormat = "log_{0:yyyyMMdd}.txt";

        // Act
        var logger = Logging.InitializeLogging(trace, logFileNameFormat);

        // Assert
        Assert.IsType<NullLogger>(logger);
    }
}