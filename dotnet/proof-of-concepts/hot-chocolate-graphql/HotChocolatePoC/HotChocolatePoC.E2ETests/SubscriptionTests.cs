using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Xunit;

namespace HotChocolatePoC.E2ETests;

public sealed class SubscriptionTests
{
    private static async Task SendAsync(WebSocket socket, object message, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(message);
        await socket.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, ct);
    }

    private static async Task<JsonDocument> ReceiveAsync(WebSocket socket, CancellationToken ct)
    {
        var buffer = new byte[16 * 1024];
        using var stream = new MemoryStream();
        WebSocketReceiveResult result;
        do
        {
            result = await socket.ReceiveAsync(buffer, ct);
            stream.Write(buffer, 0, result.Count);
        } while (!result.EndOfMessage);
        if (stream.Length == 0)
            throw new InvalidOperationException(
                $"Empty WS frame: MessageType={result.MessageType} CloseStatus={result.CloseStatus} Desc={result.CloseStatusDescription}");
        stream.Position = 0;
        return await JsonDocument.ParseAsync(stream, cancellationToken: ct);
    }

    [Fact]
    public async Task BookAddedSubscription_ReceivesEventPublishedByAddArticle()
    {
        var factory = new GraphQlTestFactory();
        await factory.InitializeAsync();
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            var wsClient = factory.Server.CreateWebSocketClient();
            wsClient.ConfigureRequest = request =>
                request.Headers["Sec-WebSocket-Protocol"] = "graphql-transport-ws";
            using var socket = await wsClient.ConnectAsync(new Uri("ws://localhost/graphql"), cts.Token);

            await SendAsync(socket, new { type = "connection_init", payload = new { } }, cts.Token);
            var ack = await ReceiveAsync(socket, cts.Token);
            Assert.Equal("connection_ack", ack.RootElement.GetProperty("type").GetString());

            await SendAsync(socket, new
            {
                id = "1",
                type = "subscribe",
                payload = new { query = "subscription { bookAdded { id articleNumber } }" }
            }, cts.Token);

            var mutation = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "mutation { addArticle(input: { article: { articleNumber: \"S-400\", status: ACTIVE, articleNumberSupplier: \"SUP-9\", ean: \"999\", kto: \"K9\", articleMatchcode: \"sub\", minimumOrderQuantity: 1, packagingUnit: 1, packaging: \"box\", purchasePrice: 9.9, variantId: 4, variantValue: \"blue\", variantType: \"color\", volume: 1, length: 1, width: 1, height: 1, outerBoxVolume: 1, outerBoxLength: 1, outerBoxWidth: 1, outerBoxHeight: 1, label1: \"a\", label2: \"b\", label3: \"c\", label4: \"d\", reorderNotice: \"r\", isNew: true } }) { articleDto { id } } }");
            var createdId = mutation.RootElement.GetProperty("data").GetProperty("addArticle").GetProperty("articleDto").GetProperty("id").GetString();

            var evt = await ReceiveAsync(socket, cts.Token);
            Assert.Equal("next", evt.RootElement.GetProperty("type").GetString());
            Assert.Equal("1", evt.RootElement.GetProperty("id").GetString());
            Assert.Equal(createdId, evt.RootElement.GetProperty("payload").GetProperty("data").GetProperty("bookAdded").GetProperty("id").GetString());

            await SendAsync(socket, new { id = "1", type = "complete" }, cts.Token);
        }
        finally
        {
            await factory.CleanupAsync();
            await factory.DisposeAsync();
        }
    }
}
