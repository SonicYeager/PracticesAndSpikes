namespace HotChocolatePoC.Database.Entities;

public class ArticleEntity
{
    public string Id { get; set; }

    public ArticleState Status { get; set; }

    public bool Eol { get; set; }

    public string? ArticleNumber { get; set; }

    public string? ArticleNumberSupplier { get; set; }

    public string? Ean { get; set; }

    public string? Kto { get; set; }

    public string? ArticleMatchcode { get; set; }

    public int MinimumOrderQuantity { get; set; }

    public int PackagingUnit { get; set; }

    public string? Packaging { get; set; }

    public decimal PurchasePrice { get; set; }

    public double? InnerBoxLength { get; set; }

    public double? InnerBoxWidth { get; set; }

    public double? InnerBoxHeight { get; set; }

    public double? OuterBoxLength { get; set; }

    public double? OuterBoxWidth { get; set; }

    public double? OuterBoxHeight { get; set; }

    public int? OuterBoxQuantity { get; set; }

    public int VariantId { get; set; }

    public string? VariantValue { get; set; }

    public string? VariantType { get; set; }

    public string? Label1 { get; set; }

    public string? Label2 { get; set; }

    public string? Label3 { get; set; }

    public string? Label4 { get; set; }

    public string? ReorderNotice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ArticleSupplierPropertiesEntity? SupplierProperties { get; set; }

    public int? ProductionSiteId { get; set; }

    public ICollection<ArticleCommentEntity> Comments { get; } = new HashSet<ArticleCommentEntity>();

    public ICollection<ArticleSimilarityEntity> SimilarEntities { get; } = new HashSet<ArticleSimilarityEntity>();

    public ICollection<ArticleSimilarityEntity> SimilarReverseEntities { get; } = new HashSet<ArticleSimilarityEntity>();

    public ICollection<ImageEntity> Images { get; } = new HashSet<ImageEntity>();

    public decimal? CustomsTariffRate { get; set; }

    public string? CustomsTariffNumber { get; set; }

    public string? CustomsMatchcode { get; set; }

    public bool IsNew { get; set; }
}