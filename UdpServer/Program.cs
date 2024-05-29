using UdpServer;
using UdpServer.MessageProcessors;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IReceivedMessageProcessor, ReceivedMessageProcessor>();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
