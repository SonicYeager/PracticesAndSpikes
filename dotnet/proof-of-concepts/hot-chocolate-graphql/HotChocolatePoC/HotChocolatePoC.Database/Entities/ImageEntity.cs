namespace HotChocolatePoC.Database.Entities
{
    public sealed class ImageEntity
    {
        public int Id { get; set; }
        public string ArticleId { get; set; }
        public string Url { get; set; }
        public ImageType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ArticleEntity? Article { get; set; }
    }
}