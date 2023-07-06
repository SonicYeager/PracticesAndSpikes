using HotChocolate.Checker.GraphQL.Types;
using HotChocolate.Checker.Persistence;
using HotChocolate.Checker.Persistence.Entities;
using HotChocolate.Resolvers;

namespace HotChocolate.Checker.GraphQL;

[QueryType]
public sealed class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> Books([Service] CheckerDbContext checkerDbContext, IResolverContext resolverContext)
    {
        return checkerDbContext.Set<BookEntity>().ProjectTo<BookEntity, Book>(resolverContext);
    }
}