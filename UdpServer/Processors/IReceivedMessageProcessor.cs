using System.Net.Sockets;

namespace UdpServer.Processors
{
    public interface IReceivedMessageProcessor
    {
        public string DecodeReceivedMessage(UdpReceiveResult receivedData);
    }
}
