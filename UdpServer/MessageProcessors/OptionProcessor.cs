using System.Text.RegularExpressions;

namespace UdpServer.MessageProcessors
{
    public class OptionProcessor : IOptionProcessor
    {
        // TODO: make it configurable
        private const string _pattern = @"^-\S+";

        public string ProcessOption(string message)
        {
            Match match = Regex.Match(message, _pattern);

            return match.Value;
        }
    }
}
