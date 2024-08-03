using TaskConsoleApp.Infrastructure;
using TaskConsoleApp.Resources;
using TaskConsoleApp.Utilities;

namespace UnitTests.Utilities
{
    public class PathFilesTests
    {
        [Fact]
        public void GetInputFilePath_ShouldReturnNull_WhenArgsIsEmptyAndFileDoesNotExist()
        {
            // Arrange
            var args = new List<string?>();
            var appConfig = new AppConfig ("nonexistentfile.txt");

            // Act
            var exception = Assert.Throws<Exception>(() => PathFiles.GetInputFilePath(args, appConfig));

            // Assert
            Assert.Equal(ConfigConstants.SrcFileNotFoundFailureMessage, exception.Message);
        }

        [Fact]
        public void GetInputFilePath_ShouldReturnFilePath_WhenArgsContainsValidPath()
        {
            // Arrange
            var args = new List<string?> { "testfile.txt" };
            var appConfig = new AppConfig("");

            // Create a dummy file
            File.WriteAllText("testfile.txt", "dummy content");

            // Act
            var result = PathFiles.GetInputFilePath(args, appConfig);

            // Assert
            Assert.Equal("testfile.txt", result);

            // Cleanup
            File.Delete("testfile.txt");
        }

        [Fact]
        public void GetInputFilePath_ShouldReturnNull_WhenArgsContainsInvalidPath()
        {
            // Arrange
            var args = new List<string?> { "invalidfile.txt" };
            var appConfig = new AppConfig("");

            // Act
            var result = PathFiles.GetInputFilePath(args, appConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetInputFilePath_ShouldReturnFilePathFromAppConfig_WhenArgsIsEmptyAndFileExists()
        {
            // Arrange
            var basePath = AppContext.BaseDirectory;
            var testFilePath = Path.Combine(basePath, "testfile.txt");
            var args = new List<string?>();
            var appConfig = new AppConfig("testfile.txt");

            // Create a dummy file
            File.WriteAllText(testFilePath, "dummy content");

            // Act
            var result = PathFiles.GetInputFilePath(args, appConfig);

            // Assert
            Assert.Equal(testFilePath, result);

            // Cleanup
            File.Delete(testFilePath);
        }

        [Fact]
        public void GetFullPathToFile_ShouldThrowException_WhenFileDoesNotExist()
        {
            // Arrange
            var appConfig = new AppConfig("nonexistentfile.txt");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => PathFiles.GetInputFilePath(new List<string?>(), appConfig));
            Assert.Equal(ConfigConstants.SrcFileNotFoundFailureMessage, exception.Message);
        }
    }
}