namespace HotChocolatePoC.Types;

public class ArticleAddDto
{
    /// <summary>
    /// Internally used Id of an article at Relaxdays.
    /// </summary>
    public string ArticleNumber { get; set; }

    /// <summary>
    /// The status of the article.
    /// </summary>
    public ArticleState Status { get; set; }

    /// <summary>
    /// Article number used by the supplier.
    /// </summary>
    public string ArticleNumberSupplier { get; set; }

    /// <summary>
    /// European Article Number. Number used in the trade for bar codes.
    /// </summary>
    public string Ean { get; set; }

    /// <summary>
    /// The internally used ID of the supplier providing the article.
    /// </summary>
    public string Kto { get; set; }

    /// <summary>
    /// Search term for an article.
    /// </summary>
    public string ArticleMatchcode { get; set; }

    /// <summary>
    /// The agreed minimum order quantity.
    /// </summary>
    public int MinimumOrderQuantity { get; set; }

    /// <summary>
    /// How many articles are packed together.
    /// </summary>
    public int PackagingUnit { get; set; }

    /// <summary>
    /// The packaging of the article.
    /// </summary>
    public string Packaging { get; set; }

    /// <summary>
    /// Standard purchase price per piece.
    /// </summary>
    public decimal PurchasePrice { get; set; }

    /// <summary>
    /// The customs matchcode
    /// </summary>
    public string? CustomsMatchcode { get; set; }

    /// <summary>
    /// The customs tariff number
    /// </summary>
    public string? CustomsTariffNumber { get; set; }

    /// <summary>
    /// The customs tariff rate
    /// </summary>
    public decimal? CustomsTariffRate { get; set; }

    /// <summary>
    /// Id of an article variant
    /// </summary>
    public int VariantId { get; set; }

    /// <summary>
    /// Name of a variant property.
    /// </summary>
    public string VariantValue { get; set; }

    /// <summary>
    /// Type of a variant property.
    /// </summary>
    public string VariantType { get; set; }

    /// <summary>
    /// Volume of an inner box in cm³.
    /// </summary>
    public double Volume { get; set; }

    /// <summary>
    /// Length of an inner box in cm.
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Width of an inner box in cm.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Height of an inner box in cm.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Volume of an outer box in cm³.
    /// </summary>
    public double OuterBoxVolume { get; set; }

    /// <summary>
    /// Length of an outer box in cm.
    /// </summary>
    public double OuterBoxLength { get; set; }

    /// <summary>
    /// Width of an outer box in cm.
    /// </summary>
    public double OuterBoxWidth { get; set; }

    /// <summary>
    /// Height of an outer box in cm.
    /// </summary>
    public double OuterBoxHeight { get; set; }

    /// <summary>
    /// Article details printed on the package label in area 1.
    /// </summary>
    public string Label1 { get; set; }

    /// <summary>
    /// Article details printed on the package label in area 2.
    /// </summary>
    public string Label2 { get; set; }

    /// <summary>
    /// Article details printed on the package label in area 3.
    /// </summary>
    public string Label3 { get; set; }

    /// <summary>
    /// Article details printed on the package label in area 4.
    /// </summary>
    public string Label4 { get; set; }

    /// <summary>
    /// Warnings or improvement suggestions in case of reorder.
    /// </summary>
    public string ReorderNotice { get; set; }

    /// <summary>
    /// Indicates whether the article is new or has been ordered already.
    /// </summary>
    public bool IsNew { get; set; }
}