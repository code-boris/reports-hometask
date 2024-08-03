using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Infrastructure;
using TaskConsoleApp.Resources;
using TaskConsoleApp.Utilities;

namespace TaskConsoleApp;

internal static class Program
{
    internal static async Task<int> Main(string?[] args)
    {
        SetDefaultCulture();
            
        var serviceProvider = Configuration.ConfigureServices();
            
        using var serviceScope = (serviceProvider ?? throw new InvalidOperationException(ConfigConstants.ConfiguringServicesFailureMessages)).CreateScope();
        var provider = serviceScope.ServiceProvider;
            
        var appConfig = provider.GetRequiredService<AppConfig>();
        var sessionService = provider.GetRequiredService<ISessionService>();

        var inputFilePath = PathFiles.GetInputFilePath(args, appConfig);

        if (string.IsNullOrEmpty(inputFilePath))
        {
            Console.WriteLine(ConfigConstants.NoInputFileFailureMessage);
            return await Task.FromException<int>(new InvalidOperationException(ConfigConstants.ErrorOccuredFailureMessage));
        }

        try
        {
            await sessionService.GenerateReportAsync(inputFilePath);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ConfigConstants.ErrorReadingFileFailureMessage, ex.Message, null);
            return await Task.FromException<int>(ex);
        }
    }

    private static void SetDefaultCulture()
    {
        Console.InputEncoding = Encoding.Default;
        Console.OutputEncoding = Encoding.Default;
    }
}