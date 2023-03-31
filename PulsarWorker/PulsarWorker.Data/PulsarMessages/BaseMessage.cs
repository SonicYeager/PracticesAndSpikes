namespace PulsarWorker.Data.PulsarMessages;
public record BaseMessage(string MessageId, DateTime ReceivedAt);