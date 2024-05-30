using System.Net.Sockets;

namespace UdpServer.MessageProcessors
{
    public interface IReceivedMessageProcessor
    {
        public string DecodeReceivedMessage(UdpReceiveResult receivedData);
    }
}
