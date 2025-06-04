namespace HotChocolatePoC.Database.Entities
{
    public class ArticleSupplierPropertiesEntity
    {
        public string ArticleId { get; set; }
        public virtual ArticleEntity? Article { get; set; }

        public double? ItemWeight { get; set; }

        public double? InnerBoxHeight { get; set; }

        public double? InnerBoxWidth { get; set; }

        public double? InnerBoxLength { get; set; }

        public double? InnerBoxWeight { get; set; }

        public double? InnerBoxWrappingBoardWeight { get; set; }

        public double? InnerBoxWrappingPlasticWeight { get; set; }

        public double? OuterBoxHeight { get; set; }

        public double? OuterBoxWidth { get; set; }

        public double? OuterBoxLength { get; set; }

        public double? OuterBoxWeight { get; set; }

        public double? OuterBoxWrappingBoardWeight { get; set; }

        public double? OuterBoxWrappingPlasticWeight { get; set; }

        public int? InnerBoxesPerOuterBox { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}