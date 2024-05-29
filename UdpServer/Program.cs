using Server;
using Server.MessageProcessors;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IReceivedMessageProcessor, ReceivedMessageProcessor>();
    })
    .Build();

await host.RunAsync();
