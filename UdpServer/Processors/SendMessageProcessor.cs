using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpServer.Processors
{
    public class SendMessageProcessor : ISendMessageProcessor
    {
        private UdpClient? _server;

        public SendMessageProcessor()
        {
        }

        public async Task SendMessage(string message, IPEndPoint remoteEndPoint, Action<string>? log, CancellationToken stoppingToken)
        {
            log?.Invoke(message);

            var sendBytes = Encoding.ASCII.GetBytes(message);
            await _server!.SendAsync(sendBytes, remoteEndPoint, stoppingToken);
        }

        public void SetServer(UdpClient server)
        {
            _server = server;
        }
    }
}
