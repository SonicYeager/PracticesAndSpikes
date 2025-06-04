namespace HotChocolatePoC.Types
{
    /// <summary>
    /// Represents a content image for an specific article.
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Url"></param>
    /// <param name="Type"></param>
    /// <param name="CreatedAt"></param>
    /// <param name="UpdatedAt"></param>
    public sealed record ImageDto(
        int Id,
        string Url,
        ImageType Type,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}