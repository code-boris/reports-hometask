using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Services;
using TaskConsoleApp.Infrastructure.Logging;
using TaskConsoleApp.Infrastructure;

namespace TaskConsoleApp
{
    internal static class Program
    {
        internal static async Task<int> Main(string[] args)
        {
            SetDefaultCulture();

            var serviceProvider = ConfigureServices();
            
            using var serviceScope = serviceProvider.CreateScope();
            var provider = serviceScope.ServiceProvider;
            
            var appConfig = provider.GetRequiredService<AppConfig>();
            var sessionService = provider.GetRequiredService<ISessionService>();
            
            var inputFilePath = args.Length > 0 ? args[0] : appConfig.InputFilePath;
            
            if (string.IsNullOrEmpty(inputFilePath))
            {
                Console.WriteLine(ConfigConstants.NoInputFileFailureMessage);
                return await Task.FromResult(0);
            }
            
            await sessionService.GenerateReportAsync(inputFilePath);
            return await Task.FromResult(1);
        }

        private static void SetDefaultCulture()
        {
            Console.InputEncoding = Encoding.Default;
            Console.OutputEncoding = Encoding.Default;
        }

        private static ServiceProvider ConfigureServices()
        {
            var configuration = LoadConfiguration();
            var appConfig = configuration.GetSection(ConfigConstants.AppConfigSection).Get<AppConfig>();
            var loggingConfig = configuration.GetSection(ConfigConstants.LogConfigSection).Get<LoggingConfig>();

            if (appConfig == null) 
                throw new InvalidOperationException(ConfigConstants.ConfigAppLoadFailureMessage);

            if (loggingConfig == null) 
                throw new InvalidOperationException(ConfigConstants.ConfigLogLoadFailureMessage);
            
            var logger = InitializeLogging(appConfig.EnableLogging, loggingConfig.LogFileNameFormat);

            // Setup Dependency Injection
            return new ServiceCollection()
                .AddSingleton(appConfig)
                .AddSingleton(logger)
                .AddSingleton<ISessionReader, CsvSessionReader>()
                .AddSingleton<ReportService>()
                .AddSingleton<ISessionService, SessionService>()
                .BuildServiceProvider();
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigConstants.AppSettingsFileName, optional: false, reloadOnChange: true);
            return builder.Build();
        }

        /// <summary>
        /// Initialize logging. It's off by default, unless the user passes the --trace flag.
        /// </summary>
        private static ITraceLogger InitializeLogging(bool trace, string logFileNameFormat) =>
            !trace ? new NullLogger() : TraceLogger.Create(string.Format(logFileNameFormat, DateTime.UtcNow));
    }
}