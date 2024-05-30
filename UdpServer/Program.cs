using UdpServer.ConsoleFormatters;
using UdpServer.MessageProcessors;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<UdpServer.UdpServer>();
        services.AddSingleton<IReceivedMessageProcessor, ReceivedMessageProcessor>();
        services.AddSingleton<ISendMessageProcessor, SendMessageProcessor>();
        services.AddSingleton<IOptionProcessor, OptionProcessor>();
        services.AddSingleton<CustomConsoleFormatterOptions>();
    })
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddConsole();
        loggingBuilder.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>();
    })
    .Build();



await host.RunAsync();
