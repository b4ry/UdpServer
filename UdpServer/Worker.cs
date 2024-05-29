using System.Net.Sockets;
using UdpServer.MessageProcessors;

namespace UdpServer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly UdpClient _udpServer;
        private readonly IReceivedMessageProcessor _receivedMessageProcessor;
        private readonly int _port;

        public Worker(ILogger<Worker> logger, IReceivedMessageProcessor receivedMessageProcessor)
        {
            _logger = logger;
            _port = int.Parse(Environment.GetEnvironmentVariable("UDP_PORT") ?? "11000");
            _udpServer = new UdpClient(_port);
            _receivedMessageProcessor = receivedMessageProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Server is listening on port {_port}...");

            stoppingToken.Register(() =>
            {
                _logger.LogInformation("Cancellation requested.\nCleaning up the resources.");

                _udpServer.Close();
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_udpServer.Available > 0) // only read when there is some data available
                    {
                        var receivedData = await _udpServer.ReceiveAsync(stoppingToken);
                        var decodedReceivedData = _receivedMessageProcessor.DecodeReceivedMessage(receivedData);

                        await Task.Delay(1000, stoppingToken); // we need this to save a bit of CPU time. Otherwise this loop will go crazy
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