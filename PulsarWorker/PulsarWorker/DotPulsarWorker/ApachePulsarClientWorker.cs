using DotPulsar;
using DotPulsar.Extensions;
using PulsarWorker.FsWorker;
using System.Buffers;
using System.Text;

namespace PulsarWorker.DotPulsarWorker;

public class ApachePulsarClientWorker : BackgroundService
{
    private readonly ILogger<FSPulsarClientWorker> _logger;

    public ApachePulsarClientWorker(ILogger<FSPulsarClientWorker> logger)
    {
        _logger = logger;
    }

    private async Task RunRealWorld()
    {
        await using var client = PulsarClient.Builder()
            .Build();

        await using var producer = client.NewProducer()
            .Topic("persistent://public/default/mytopic")
            .Create();

        await using var consumer = client.NewConsumer()
            .SubscriptionName("MySubscription")
            .Topic("persistent://public/default/mytopic")
            .Create();

        var data = "Hello World"u8.ToArray();
        await producer.Send(data);

        await foreach (var message in consumer.Messages())
        {
            Console.WriteLine("Received: " + Encoding.UTF8.GetString(message.Data.ToArray()));
            await consumer.Acknowledge(message);
            break;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunRealWorld();
            await Task.Delay(10000, stoppingToken);
        }
    }
}