namespace PulsarWorker.Data;

public sealed record BaseMessage(string MessageId, DateTime SendAt);