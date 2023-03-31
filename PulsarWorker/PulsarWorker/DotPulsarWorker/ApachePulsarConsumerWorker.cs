using DotPulsar;
using DotPulsar.Extensions;
using PulsarWorker.Data;
using System.Buffers;
using System.Text;

namespace PulsarWorker.DotPulsarWorker;

public class ApachePulsarConsumerWorker : BackgroundService
{
    private static async Task RunRealWorld(CancellationToken cancellationToken)
    {
        await using var client = PulsarClient.Builder()
            .Build();

        await using var consumer = client.NewConsumer(new JsonSchema<BaseMessage>())
            .SubscriptionName("MySubscription")
            .Topic("persistent://public/default/mytopic")
            .Create();

        await foreach (var message in consumer.Messages(cancellationToken))
        {
            Console.WriteLine("Received: " + message.Value().MessageId);
            await consumer.Acknowledge(message, cancellationToken: cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunRealWorld(stoppingToken);
        }
    }
}