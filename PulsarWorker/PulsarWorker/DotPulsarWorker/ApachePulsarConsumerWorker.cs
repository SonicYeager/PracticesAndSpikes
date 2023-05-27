using System.Text.Json;
using DotPulsar;
using DotPulsar.Extensions;
using BaseMessage = PulsarWorker.Data.BaseMessage;

namespace PulsarWorker.DotPulsarWorker;

public class ApachePulsarConsumerWorker : BackgroundService
{
    private static async Task RunRealWorld(CancellationToken cancellationToken)
    {
        await using var client = PulsarClient.Builder().ServiceUrl(new("pulsar://localhost:6650"))
            .Build();

        await using var consumer = client.NewConsumer(Schema.String)
            .SubscriptionName("MySubscription")
            .Topic("persistent://public/default/mytopic")
            .Create();

        await foreach (var message in consumer.Messages(cancellationToken))
        {
            var decoded = JsonSerializer.Deserialize<BaseMessage>(message.Value());
            
            Console.WriteLine("Received: " + decoded?.MessageId);
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