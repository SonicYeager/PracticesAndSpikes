namespace HotChocolatePoC.Types
{
    /// <summary>
    /// Represents whether the article is active or not.
    /// </summary>
    public enum ArticleState
    {
        /// <summary>
        /// Article is actively in use.
        /// </summary>
        Active = 10,

        /// <summary>
        /// Article is no longer used.
        /// </summary>
        Inactive = 20
    }
}