using Moq;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Resources;
using TaskConsoleApp.Utilities;

namespace UnitTests.Utilities
{
    public class ReportsTests
    {
        private readonly Mock<ITraceLogger> _mockLogger = new();

        [Fact]
        public void GetColumnWidths_ShouldReturnCorrectWidths()
        {
            // Arrange
            var operatorStates = new Dictionary<string, Dictionary<string, TimeSpan>>
            {
                { "Иванов Иван Иванович", new Dictionary<string, TimeSpan>
                    {
                        { StringConstants.Pause, TimeSpan.FromSeconds(30) },
                        { StringConstants.Ready, TimeSpan.FromSeconds(45) },
                        { StringConstants.Talk, TimeSpan.FromSeconds(60) },
                        { StringConstants.Processing, TimeSpan.FromSeconds(75) },
                        { StringConstants.Recall, TimeSpan.FromSeconds(90) }
                    }
                },
                { "Петров Петр Петрович", new Dictionary<string, TimeSpan>
                    {
                        { StringConstants.Pause, TimeSpan.FromSeconds(40) },
                        { StringConstants.Ready, TimeSpan.FromSeconds(50) },
                        { StringConstants.Talk, TimeSpan.FromSeconds(70) },
                        { StringConstants.Processing, TimeSpan.FromSeconds(80) },
                        { StringConstants.Recall, TimeSpan.FromSeconds(100) }
                    }
                }
            };

            // Act
            var result = Reports.GetColumnWidths(operatorStates);

            // Assert
            Assert.Equal(20, result.nameWidth);
            Assert.Equal(5, result.pauseWidth); 
            Assert.Equal(5, result.readyWidth); 
            Assert.Equal(8, result.talkWidth); 
            Assert.Equal(9, result.processingWidth); 
            Assert.Equal(8, result.recallWidth);
        }
        
        [Fact]
        public void PrintSecondReportHeader_ShouldPrintCorrectly()
        {
            // Arrange
            var columnWidths = (19, 5, 5, 5, 5, 5); // Example widths
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            Reports.PrintSecondReportHeader(columnWidths);

            // Assert
            var expectedOutput = "ФИО                 Пауза Готов Разговор Обработка Перезвон\r\n";
            Assert.Equal(expectedOutput, output.ToString());
        }

        [Fact]
        public void PrintSecondReport_ShouldPrintCorrectly()
        {
            // Arrange
            var operatorStates = new Dictionary<string, Dictionary<string, TimeSpan>>
            {
                { "Иванов Иван Иванович", new Dictionary<string, TimeSpan>
                    {
                        { StringConstants.Pause, TimeSpan.FromSeconds(30) },
                        { StringConstants.Ready, TimeSpan.FromSeconds(45) },
                        { StringConstants.Talk, TimeSpan.FromSeconds(60) },
                        { StringConstants.Processing, TimeSpan.FromSeconds(75) },
                        { StringConstants.Recall, TimeSpan.FromSeconds(90) }
                    }
                }
            };

            var columnWidths = (19, 5, 5, 5, 5, 5); // Example widths
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            Reports.PrintSecondReport(operatorStates, columnWidths);

            // Assert
            var expectedOutput = "Иванов Иван Иванович    30    45    60    75    90\r\n\r\n";
            Assert.Equal(expectedOutput, output.ToString());
        }

        [Fact]
        public void PrintSecondReport_ShouldPrintInRed_WhenNameIsInvalid()
        {
            // Arrange
            var operatorStates = new Dictionary<string, Dictionary<string, TimeSpan>>
            {
                { "InvalidName", new Dictionary<string, TimeSpan>
                    {
                        { StringConstants.Pause, TimeSpan.FromSeconds(30) },
                        { StringConstants.Ready, TimeSpan.FromSeconds(45) },
                        { StringConstants.Talk, TimeSpan.FromSeconds(60) },
                        { StringConstants.Processing, TimeSpan.FromSeconds(75) },
                        { StringConstants.Recall, TimeSpan.FromSeconds(90) }
                    }
                }
            };

            var columnWidths = (11, 5, 5, 5, 5, 5); // Example widths
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            Reports.PrintSecondReport(operatorStates, columnWidths);

            // Assert
            // Note: Foreground color change doesn't affect string output, only console color.
            var expectedOutput = "InvalidName    30    45    60    75    90\r\n\r\n";
            Assert.Equal(expectedOutput, output.ToString());
        }
    }
}
