namespace HotChocolatePoC.Database.Entities
{
    public class ArticleCommentEntity
    {
        public int Id { get; set; }

        public string ArticleId { get; set; }
        public virtual ArticleEntity? Article { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedById { get; set; }
    }
}