using PulsarWorker.DotPulsarWorker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ApachePulsarProducerWorker>();
    })
    .Build();

host.Run();