using System.Net.Sockets;

namespace Server.MessageProcessors
{
    public interface IReceivedMessageProcessor
    {
        public string DecodeReceivedMessage(UdpReceiveResult receivedData);
    }
}
