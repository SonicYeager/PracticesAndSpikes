namespace HotChocolatePoC.Types
{
    /// <summary>
    /// Contains all fields that represent an outer box.
    /// </summary>
    public sealed class OuterBoxDto : ArticleBoxDto
    {
        /// <summary>
        /// The number of inner boxes per outer box.
        /// </summary>
        public int? InnerBoxesPerOuterBox { get; set; }
    }
}