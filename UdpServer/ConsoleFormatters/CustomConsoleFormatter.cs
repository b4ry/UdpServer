using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System.Globalization;

namespace UdpServer.ConsoleFormatters
{
    public class CustomConsoleFormatter : ConsoleFormatter, IDisposable
    {
        CustomConsoleFormatterOptions _options;

        public CustomConsoleFormatter(CustomConsoleFormatterOptions options) : base("Custom")
        {
            _options = options;
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            var logLevel = logEntry.LogLevel;

            switch (logLevel)
            {
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.White;
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
}
