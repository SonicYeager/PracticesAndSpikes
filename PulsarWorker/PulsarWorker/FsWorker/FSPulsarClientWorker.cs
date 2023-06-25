using Pulsar.Client.Api;
using Pulsar.Client.Common;
using System.Text;

namespace PulsarWorker.FsWorker;

public class FSPulsarClientWorker : BackgroundService
{
    private readonly ILogger<FSPulsarClientWorker> _logger;
    //private PulsarClient? _pulsarClient;

    public FSPulsarClientWorker(ILogger<FSPulsarClientWorker> logger)
    {
        _logger = logger;
    }

    private static async Task SendMessage(IProducer<byte[]> producer, ILogger logger, byte[] message)
    {
        try
        {
            var messageId = await producer.SendAsync(message);
            logger.LogInformation("MessageSent to {0}. Id={1}", producer.Topic, messageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error on produce message to {0}", producer.Topic);
        }
    }

    internal static async Task ProcessMessages(IConsumer<byte[]> consumer, ILogger logger,
        Func<Message<byte[]>, Task> f,
        CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                var success = false;
                var message = await consumer.ReceiveAsync();
                try
                {
                    await f(message);
                    success = true;
                    logger.LogDebug("Message handled");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Can't process message {0}, Id={1}", consumer.Topic, message.MessageId);
                    await consumer.NegativeAcknowledge(message.MessageId);
                }

                if (success)
                {
                    await consumer.AcknowledgeAsync(message.MessageId);
                    logger.LogDebug("Message acknowledged");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ProcessMessages failed for {0}", consumer.Topic);
        }
    }


    internal static async Task RunRealWorld(ILogger logger)
    {
        const string serviceUrl = "pulsar://localhost:6650";
        const string subscriptionName = "my-subscription";
        var topicName = $"my-topic-{DateTime.Now.Ticks}";

        var client = await new PulsarClientBuilder()
            .ServiceUrl(serviceUrl)
            .BuildAsync();

        var producer = await client.NewProducer()
            .Topic(topicName)
            .EnableBatching(false)
            .CreateAsync();

        var consumer = await client.NewConsumer()
            .Topic(topicName)
            .SubscriptionName(subscriptionName)
            .SubscribeAsync();

        var cts = new CancellationTokenSource();
        await Task.Run(() => ProcessMessages(consumer, logger, (message) =>
        {
            var messageText = Encoding.UTF8.GetString(message.Data);
            logger.LogInformation("Received: {MessageText}", messageText);
            return Task.CompletedTask;
        }, cts.Token), cts.Token);

        for (var i = 0; i < 100; i++)
        {
            await SendMessage(producer, logger, Encoding.UTF8.GetBytes($"Sent from C# at {DateTime.Now} message #{i}"));
        }

        cts.Dispose();
        await Task.Delay(200, cts.Token); // wait for pending acknowledgments to complete
        await consumer.DisposeAsync();
        await producer.DisposeAsync();
        await client.CloseAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PulsarClient.Logger = _logger;
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
            await RunRealWorld(_logger);
            await Task.Delay(10000, stoppingToken);
        }
    }
}