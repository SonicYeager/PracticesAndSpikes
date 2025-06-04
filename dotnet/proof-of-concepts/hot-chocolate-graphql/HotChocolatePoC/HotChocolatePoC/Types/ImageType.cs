namespace HotChocolatePoC.Types
{
    /// <summary>
    /// Represents the possible media types provided by the content api.
    /// Changes here should also be applied to "Extraworld.Articles.Database.Core.Entities.ImageType" and vice versa.
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// Represents additional images, that may be of any type.
        /// </summary>
        AdditionalImage = 0,

        /// <summary>
        /// Represents a preview image, smaller in memory size.
        /// </summary>
        Preview = 1,

        /// <summary>
        /// Represents the article in a given environment.
        /// </summary>
        Ambient = 2,

        /// <summary>
        /// Represents the article from the front.
        /// </summary>
        FrontView = 3,

        /// <summary>
        /// Represents the sizes of the article.
        /// </summary>
        ProductSpecification = 4,

        /// <summary>
        /// Represents the material if the article.
        /// </summary>
        Material = 5,
    }
}