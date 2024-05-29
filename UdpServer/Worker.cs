using System.Net.Sockets;
using System.Text;

namespace UdpServer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly UdpClient _udpServer;
        private readonly int _port;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _port = 11000;
            _udpServer = new UdpClient(_port);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Server is listening on port {_port}...");

            stoppingToken.Register(() =>
            {
                _logger.LogInformation("Cancellation requested. Cleaning up the resources.");

                _udpServer.Close();
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_udpServer.Available > 0) // only read when there is some data available
                    {
                        var receivedData = await _udpServer.ReceiveAsync(stoppingToken);
                        string decodedReceivedData = Encoding.ASCII.GetString(receivedData.Buffer);

                        _logger.LogInformation($"Received {decodedReceivedData} from {receivedData.RemoteEndPoint}");

                        await Task.Delay(1000, stoppingToken);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "An exception occured in the server");
                }
            }

            _logger.LogInformation("Server got stopped.");
        }
    }
}