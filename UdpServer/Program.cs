using UdpServer.MessageProcessors;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<UdpServer.UdpServer>();
        services.AddSingleton<IReceivedMessageProcessor, ReceivedMessageProcessor>();
        services.AddSingleton<ISendMessageProcessor, SendMessageProcessor>();
    })
    .Build();

await host.RunAsync();
