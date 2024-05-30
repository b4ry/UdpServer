using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
using UdpServer.MessageProcessors;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<UdpServer.UdpServer>();
        services.AddSingleton<IReceivedMessageProcessor, ReceivedMessageProcessor>();
        services.AddSingleton<ISendMessageProcessor, SendMessageProcessor>();
        services.AddSingleton<CustomConsoleFormatterOptions>();
    })
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>();
        loggingBuilder.AddConsole(options =>
        {
            options.FormatterName = "custom";
        });
    })
    .Build();



await host.RunAsync();

public class CustomConsoleFormatterOptions : Microsoft.Extensions.Logging.Console.ConsoleFormatterOptions
{
    public CustomConsoleFormatterOptions()
    {
        TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff zzz";
    }
}

public class CustomConsoleFormatter : Microsoft.Extensions.Logging.Console.ConsoleFormatter, IDisposable
{
    CustomConsoleFormatterOptions _options;

    public CustomConsoleFormatter(CustomConsoleFormatterOptions options) : base("custom")
    {
        _options = options;
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        var logLevel = logEntry.LogLevel;

        switch (logLevel)
        {
            case LogLevel.Information:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                break;
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
        }

        textWriter.WriteLine(DateTime.Now.ToLocalTime().ToString(_options.TimestampFormat, CultureInfo.InvariantCulture));
        textWriter.WriteLine($"{logEntry.LogLevel}: {logEntry.Formatter!(logEntry.State, logEntry.Exception)}");
    }

    public void Dispose()
    {
    }
}
