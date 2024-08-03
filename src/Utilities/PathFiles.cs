using TaskConsoleApp.Infrastructure;
using TaskConsoleApp.Resources;

namespace TaskConsoleApp.Utilities;

public static class PathFiles
{
    public static string? GetInputFilePath(IReadOnlyList<string?> args, AppConfig appConfig)
    {
        var inputFilePath = args.Count > 0 ? args[0] : GetFullPathToFile(appConfig);
            
        return File.Exists(inputFilePath) ? inputFilePath : null;
    }

    private static string GetFullPathToFile(AppConfig appConfig)
    {
        var basePath = AppContext.BaseDirectory;
        var directoryInfo = new DirectoryInfo(basePath);

        while (directoryInfo != null)
        {
            var assetsPath = Path.Combine(directoryInfo.FullName, appConfig.InputFilePath);

            if (File.Exists(assetsPath))
                return assetsPath;

            directoryInfo = directoryInfo.Parent;
        }

        throw new Exception(ConfigConstants.SrcFileNotFoundFailureMessage);
    }
}