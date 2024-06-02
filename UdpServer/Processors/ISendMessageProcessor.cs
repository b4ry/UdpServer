using System.Net;
using System.Net.Sockets;

namespace UdpServer.Processors
{
    public interface ISendMessageProcessor
    {
        public Task SendMessage(string message, IPEndPoint remoteEndPoint, Action<string>? log, CancellationToken stoppingToken);
        public void SetServer(UdpClient server);
    }
}
