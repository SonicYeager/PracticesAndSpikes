using PulsarWorker.DotPulsarWorker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ApachePulsarProducerWorker>();
        services.AddHostedService<ApachePulsarConsumerWorker>();
        //services.AddHostedService<FSPulsarClientWorker>();
    })
    .Build();

host.Run();