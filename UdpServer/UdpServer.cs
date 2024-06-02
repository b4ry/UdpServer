using System.Net;
using System.Net.Sockets;
using System.Text;
using UdpServer.Menu;
using UdpServer.Processors;

namespace UdpServer
{
    public class UdpServer : BackgroundService
    {
        private const string UdpPortEnvironmentVariableName = "UDP_PORT";

        private readonly ILogger<UdpServer> _logger;
        private readonly IReceivedMessageProcessor _receivedMessageProcessor;
        private readonly ISendMessageProcessor _sendMessageProcessor;
        private readonly IOptionProcessor _optionProcessor;

        private UdpClient? _server;

        private readonly int _port;

        private readonly Dictionary<string, IPEndPoint> _users = new();

        public UdpServer(
            ILogger<UdpServer> logger,
            IReceivedMessageProcessor receivedMessageProcessor, 
            ISendMessageProcessor sendMessageProcessor,
            IOptionProcessor optionProcessor)
        {
            _logger = logger;

            _port = int.Parse(Environment.GetEnvironmentVariable(UdpPortEnvironmentVariableName) ?? "11000");

            _receivedMessageProcessor = receivedMessageProcessor;
            _sendMessageProcessor = sendMessageProcessor;
            _optionProcessor = optionProcessor;
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
                            var option = _optionProcessor.ProcessOption(decodedReceivedData);

                            var splitData = decodedReceivedData.Split(option);
                            string message = string.Empty;

                            switch (option)
                            {
                                case MenuOptions.RegisterUser:
                                    var userNick = splitData[1].Trim();

                                    if (!_users.ContainsKey(userNick))
                                    {
                                        _users.Add(userNick, receivedData.RemoteEndPoint);

                                        message = $"Registered {userNick} : {receivedData.RemoteEndPoint}";
                                        await _sendMessageProcessor.SendMessage(
                                            message,
                                            receivedData.RemoteEndPoint,
                                            (string x) => { _logger.LogInformation(x); },
                                            stoppingToken);
                                    }
                                    else
                                    {
                                        message = $"RegistrationFailed: {userNick} already exists.";
                                        await _sendMessageProcessor.SendMessage(
                                            message,
                                            receivedData.RemoteEndPoint,
                                            (string x) => { _logger.LogWarning(x); },
                                            stoppingToken);
                                    }

                                    break;
                                case MenuOptions.ListUsers:
                                    StringBuilder usersList = new StringBuilder();

                                    foreach(var user in _users)
                                    {
                                        usersList.Append(user.Key + ":");
                                    }

                                    usersList.Remove(usersList.Length - 1, 1);

                                    await _sendMessageProcessor.SendMessage(usersList.ToString(), receivedData.RemoteEndPoint, null, stoppingToken);

                                    break;
                                case MenuOptions.DirectMessage:
                                    var lastIndexHyphen = splitData[1].LastIndexOf("-");
                                    var recipientUser = splitData[1][(lastIndexHyphen+1)..].Trim();

                                    message = $"({receivedData.RemoteEndPoint}) {splitData[1][..lastIndexHyphen]}";
                                    await _sendMessageProcessor.SendMessage(
                                        message,
                                        _users[recipientUser],
                                        null,
                                        stoppingToken);

                                    break;
                                default:
                                    _logger.LogWarning($"Unsupported command from {receivedData.RemoteEndPoint}");

                                    break;
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