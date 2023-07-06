namespace HotChocolate.Checker.Persistence.Entities;

public sealed class BookEntity
{
    public int Id { get; set; } // Unique Identifier for the book
    public string Title { get; set; } // Title of the book
    public UserEntity? Author { get; set; } // Author of the book
    public int AuthorId { get; set; }
    public DateTime PublicationDate { get; set; } // Date when the book was published
    public string ISBN { get; set; } // International Standard Book Number
    public string Genre { get; set; } // Genre of the book
    public int PageCount { get; set; } // Number of pages in the book
    public string Language { get; set; } // Language in which the book is written
}