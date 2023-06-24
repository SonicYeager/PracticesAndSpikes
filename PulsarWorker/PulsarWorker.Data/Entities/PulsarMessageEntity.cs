namespace PulsarWorker.Data.Entities;

public sealed class PulsarMessageEntity
{
    public string MessageId { get; set; } = null!;
    public DateTime ReceivedAt { get; set; }
    //public string MessageContentId { get; set; }
}