using System.Net.Sockets;
using System.Text;

namespace Server.MessageProcessors
{
    public class ReceivedMessageProcessor : IReceivedMessageProcessor
    {
        private readonly ILogger<ReceivedMessageProcessor> _logger;

        public ReceivedMessageProcessor(ILogger<ReceivedMessageProcessor> logger)
        {
            _logger = logger;
        }

        public string DecodeReceivedMessage(UdpReceiveResult receivedData)
        {
            string decodedReceivedData = Encoding.ASCII.GetString(receivedData.Buffer);

            _logger.LogInformation($"Received {decodedReceivedData} from {receivedData.RemoteEndPoint}");

            return decodedReceivedData;
        }
    }
}
