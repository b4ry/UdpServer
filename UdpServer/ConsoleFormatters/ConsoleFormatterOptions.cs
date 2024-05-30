using Microsoft.Extensions.Logging.Console;

namespace UdpServer.ConsoleFormatters
{
    public class CustomConsoleFormatterOptions : ConsoleFormatterOptions
    {
        public CustomConsoleFormatterOptions()
        {
            TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff zzz";
        }
    }
}
