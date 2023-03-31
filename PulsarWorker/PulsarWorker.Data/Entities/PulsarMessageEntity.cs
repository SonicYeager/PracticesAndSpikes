namespace PulsarWorker.Data.Entities
{
    public class PulsarMessageEntity
    {
        public string MessageId { get; set; }
        public DateTime ReceivedAt { get; set; }
        //public string MessageContentId { get; set; }
    }
}
