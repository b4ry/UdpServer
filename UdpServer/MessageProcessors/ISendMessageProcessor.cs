using System.Net;
using System.Net.Sockets;

namespace UdpServer.MessageProcessors
{
    public interface ISendMessageProcessor
    {
        public Task SendMessage(string message, IPEndPoint remoteEndPoint, bool logMessage, CancellationToken stoppingToken);
        public void SetServer(UdpClient server);
    }
}
