using OpenTelemetry.Trace;
using PulsarWorker.DotPulsarWorker;
using PulsarWorker.FsWorker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //services.AddOpenTelemetry().WithTracing(builder =>
        //{
        //    builder.AddConsoleExporter();
        //    builder.AddSource("DotPulsar");
        //});

        //services.AddHostedService<ApachePulsarClientWorker>();
        services.AddHostedService<FSPulsarClientWorker>();
    })
    .Build();

host.Run();