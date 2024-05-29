using System.Net.Sockets;
using Server.MessageProcessors;

namespace Server
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly UdpClient _server;
        private readonly IReceivedMessageProcessor _receivedMessageProcessor;
        private readonly int _port;

        private const string UdpPortEnvironmentVariableName = "UDP_PORT";

        public Worker(ILogger<Worker> logger, IReceivedMessageProcessor receivedMessageProcessor)
        {
            _logger = logger;
            _port = int.Parse(Environment.GetEnvironmentVariable(UdpPortEnvironmentVariableName) ?? "11000");
            _server = new UdpClient(_port);
            _receivedMessageProcessor = receivedMessageProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Server is listening on port {_port}...");

            stoppingToken.Register(() =>
            {
                _logger.LogInformation("Cancellation requested.\nCleaning up the resources.");

                _server.Close();
            });

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_server.Available > 0) // only read when there is some data available
                    {
                        var receivedData = await _server.ReceiveAsync(stoppingToken);
                        var decodedReceivedData = _receivedMessageProcessor.DecodeReceivedMessage(receivedData);

                        await Task.Delay(1000, stoppingToken); // we need this to save a bit of CPU time. Otherwise this loop will go crazy
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured in the server");
            }
            finally
            {
                _server.Close();
            }

            _logger.LogInformation("Server got stopped.");
        }
    }
}