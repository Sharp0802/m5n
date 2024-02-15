using Microsoft.Extensions.Logging;

namespace M5N.Logging;

public static class LoggerFactory
{
    private static readonly ILoggerFactory Factory;
    
    static LoggerFactory()
    {
        Factory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddFilter("System", LogLevel.Warning)
                   .AddFilter("Microsoft", LogLevel.Warning)
                   .AddFilter(null, LogLevel.Trace)
                   .AddConsole();
        });
    }

    public static ILogger<T> Acquire<T>() => Factory.CreateLogger<T>();
}