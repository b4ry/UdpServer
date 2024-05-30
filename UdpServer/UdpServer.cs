using System.Net;
using System.Net.Sockets;
using System.Text;
using UdpServer.MessageProcessors;

namespace UdpServer
{
    public class UdpServer : BackgroundService
    {
        private const string UdpPortEnvironmentVariableName = "UDP_PORT";

        private readonly ILogger<UdpServer> _logger;
        private readonly IReceivedMessageProcessor _receivedMessageProcessor;
        private readonly ISendMessageProcessor _sendMessageProcessor;

        private UdpClient? _server;

        private readonly int _port;

        private readonly Dictionary<string, IPEndPoint> _users = new();

        public UdpServer(ILogger<UdpServer> logger, IReceivedMessageProcessor receivedMessageProcessor, ISendMessageProcessor sendMessageProcessor)
        {
            _logger = logger;

            _port = int.Parse(Environment.GetEnvironmentVariable(UdpPortEnvironmentVariableName) ?? "11000");

            _receivedMessageProcessor = receivedMessageProcessor;
            _sendMessageProcessor = sendMessageProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (_server = new UdpClient(_port))
            {
                _sendMessageProcessor.SetServer(_server);
                _logger.LogInformation($"Server is listening on port {_port}...");

                stoppingToken.Register(() =>
                {
                    _logger.LogWarning("Cancellation requested.\nCleaning up the resources.");
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
                            var splitData = decodedReceivedData.Split(":");

                            switch (splitData[0])
                            {
                                case "UR":
                                    var userNick = splitData[1];

                                    if (!_users.ContainsKey(userNick))
                                    {
                                        _users.Add(userNick, receivedData.RemoteEndPoint);

                                        string message = $"Registered {userNick} : {receivedData.RemoteEndPoint}";
                                        _logger.LogInformation(message);

                                        await _sendMessageProcessor.SendMessage(message, receivedData.RemoteEndPoint, stoppingToken);
                                    }
                                    else
                                    {
                                        string message = $"RegistrationFailed: {userNick} already exists.";
                                        _logger.LogWarning(message);

                                        await _sendMessageProcessor.SendMessage(message, receivedData.RemoteEndPoint, stoppingToken);
                                    }

                                    break;
                                case "LU":
                                    StringBuilder usersList = new StringBuilder();

                                    foreach(var user in _users)
                                    {
                                        usersList.Append(user.Key + ":");
                                    }

                                    usersList.Remove(usersList.Length - 1, 1);

                                    await _sendMessageProcessor.SendMessage(usersList.ToString(), receivedData.RemoteEndPoint, stoppingToken);
                                    break;
                                case "DEFAULT":
                                    throw new NotImplementedException();
                            }

                            await Task.Delay(1000, stoppingToken); // we need this to save a bit of CPU time. Otherwise, this loop will go crazy
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occured in the server.");
                }

                _logger.LogInformation("Server got stopped.");
            }
        }
    }
}