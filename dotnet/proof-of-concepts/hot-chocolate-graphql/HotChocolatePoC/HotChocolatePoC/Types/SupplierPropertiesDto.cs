namespace HotChocolatePoC.Types
{
    /// <summary>
    /// The article properties given by the supplier
    /// </summary>
    public sealed class SupplierPropertiesDto
    {
        /// <summary>
        /// The weight of the item in kg.
        /// </summary>
        public double? ItemWeight { get; set; }

        /// <summary>
        /// The dimensions and weights of the inner box.
        /// </summary>
        public ArticleBoxDto? InnerBox { get; set; }

        /// <summary>
        /// The dimensions and weights of the outer box.
        /// </summary>
        public OuterBoxDto? OuterBox { get; set; }

        /// <summary>
        /// Created timestamp of these supplier properties.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Updated timestamp of these supplier properties.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}