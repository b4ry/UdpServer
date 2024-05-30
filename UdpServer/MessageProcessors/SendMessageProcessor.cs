using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpServer.MessageProcessors
{
    public class SendMessageProcessor : ISendMessageProcessor
    {
        private readonly ILogger<SendMessageProcessor> _logger;
        private UdpClient? _server;

        public SendMessageProcessor(ILogger<SendMessageProcessor> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(string message, IPEndPoint remoteEndPoint, bool logMessage, CancellationToken stoppingToken)
        {
            if (logMessage)
            {
                _logger.LogInformation(message);
            }

            var sendBytes = Encoding.ASCII.GetBytes(message);
            await _server!.SendAsync(sendBytes, remoteEndPoint, stoppingToken);
        }

        public void SetServer(UdpClient server)
        {
            _server = server;
        }
    }
}
