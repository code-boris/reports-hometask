using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskConsoleApp.Infrastructure;
using TaskConsoleApp.Interfaces;
using TaskConsoleApp.Resources;
using TaskConsoleApp.Services;
using TaskConsoleApp.Utilities;

namespace TaskConsoleApp;

/// <summary>
/// Конфигурация
/// </summary>
public static class Configuration
{
    /// <summary>
    /// Настроить сервисы
    /// </summary>
    /// <returns></returns>
    public static ServiceProvider? ConfigureServices()
    {
        var configuration = LoadConfiguration();
        var appConfig = configuration.GetSection(ConfigConstants.AppConfigSection).Get<AppConfig>();
        var loggingConfig = configuration.GetSection(ConfigConstants.LogConfigSection).Get<LoggingConfig>();

        if (appConfig == null || loggingConfig == null) return null;
        ValidateConfiguration(appConfig, loggingConfig);

        var logger = Logging.InitializeLogging(appConfig.EnableLogging, loggingConfig.LogFileNameFormat);

        return new ServiceCollection()
            .AddSingleton(appConfig)
            .AddSingleton(logger)
            .AddSingleton<ISessionReader, CsvSessionReader>()
            .AddSingleton<IReportService, ReportService>()
            .AddSingleton<ISessionService, SessionService>()
            .BuildServiceProvider();
    }
    
    /// <summary>
    /// Загрузить конфигурация из appsettings.json файла
    /// </summary>
    /// <returns>Конфигурация</returns>
    private static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ConfigConstants.AppSettingsFileName, optional: false, reloadOnChange: true);
        return builder.Build();
    }

    /// <summary>
    /// Валидировать конфигурацию
    /// </summary>
    /// <param name="appConfig">Конфигурация приложения</param>
    /// <param name="loggingConfig">Конфигурация логгирования</param>
    /// <exception cref="InvalidOperationException">Исключение, в случае, если файлов конфигурации нет</exception>
    private static void ValidateConfiguration(AppConfig appConfig, LoggingConfig loggingConfig)
    {
        if (appConfig == null) 
            throw new InvalidOperationException(ConfigConstants.ConfigAppLoadFailureMessage);

        if (loggingConfig == null) 
            throw new InvalidOperationException(ConfigConstants.ConfigLogLoadFailureMessage);
    }
}

