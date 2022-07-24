using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class AppInsightsLog
{
    #region Methods

    public static ILoggingBuilder AddAppInsightsFilter(this ILoggingBuilder builder)
    {
        builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
        return builder;
    }

    /// <summary>
    /// Add ApplicationInsights Logs
    /// </summary>
    /// <param name="services"></param>
    /// <param name="instrumentationKey">If this is Null the key will be pickup from ApplicationInsights section of appsettings.json</param>
    /// <returns></returns>
    public static IServiceCollection AddAppInsightsLog(this IServiceCollection services, string instrumentationKey = null)
    {
        if (string.IsNullOrEmpty(instrumentationKey))
            services.AddApplicationInsightsTelemetry();
        else services.AddApplicationInsightsTelemetry(instrumentationKey);

        services.ConfigureTelemetryModule<PerformanceCollectorModule>((module, o) =>
        {
            // the application process name could be "dotnet" for ASP.NET Core self-hosted applications.
            module.Counters.Add(new PerformanceCounterCollectionRequest(@"\Process([replace-with-application-process-name])\Page Faults/sec", "DotnetPageFaultsPerfSec"));
        });

        return services;
    }

    public static IHostBuilder UseAppInsightsLog(this IHostBuilder builder, Action<ILoggingBuilder> config = null)
    {
        builder.ConfigureLogging(b => { b.AddAppInsightsFilter(); config?.Invoke(b); });
        return builder;
    }

    #endregion Methods
}