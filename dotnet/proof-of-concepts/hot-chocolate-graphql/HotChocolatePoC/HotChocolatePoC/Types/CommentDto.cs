namespace HotChocolatePoC.Types
{
    /// <summary>
    /// Represents comments.
    /// </summary>
    public class CommentDto
    {
        /// <summary>
        /// The identifier of a comment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The content of the comment.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The creation date and time.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}