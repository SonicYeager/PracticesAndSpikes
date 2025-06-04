namespace HotChocolatePoC.Database.Entities
{
    public class ArticleSimilarityEntity
    {
        public string ArticleId { get; set; }
        public virtual ArticleEntity? Article { get; set; }

        public string SimilarArticleId { get; set; }
        public virtual ArticleEntity? SimilarArticle { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedById { get; set; }
    }
}