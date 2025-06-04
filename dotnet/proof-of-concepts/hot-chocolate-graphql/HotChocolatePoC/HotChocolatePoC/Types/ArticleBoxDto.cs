namespace HotChocolatePoC.Types
{
    /// <summary>
    /// The dimensions and the weights of an article box
    /// </summary>
    public class ArticleBoxDto
    {
        /// <summary>
        /// The dimensions of the article box.
        /// </summary>
        public DimensionsDto? Dimensions { get; set; }

        /// <summary>
        /// The weight of the article box in kg.
        /// </summary>
        public double? BoxWeight { get; set; }

        /// <summary>
        /// The weight of the wrapping board in kg.
        /// </summary>
        public double? WrappingBoardWeight { get; set; }

        /// <summary>
        /// The weight of the wrapping plastic in kg.
        /// </summary>
        public double? WrappingPlasticWeight { get; set; }
    }
}