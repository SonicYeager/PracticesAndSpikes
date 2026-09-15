using AutoMapper;
using HotChocolate.Authorization;
using HotChocolatePoC.Database.Context;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.DataLoaders;
using HotChocolatePoC.Types;
using Microsoft.EntityFrameworkCore;

namespace HotChocolatePoC.QueryTypes;

public class ArticleQuery
{
    [UseProjection]
    public ArticleDto GetArticle(string id, [Service] IDbContextFactory<ArticlesDbContext> factory, [Service] IMapper mapper)
    {
        using var context = factory.CreateDbContext();
        var entity = context.Articles
            .Includings()
            .SingleOrDefault(i => i.Id == id)
            ?? throw new System.Collections.Generic.KeyNotFoundException($"Article '{id}' not found.");
        return mapper.Map<ArticleDto>(entity);
    }

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 20, MaxPageSize = 100)]
    [UseProjection]
    [HotChocolate.Data.UseFiltering]
    [UseSorting]
    public IEnumerable<ArticleDto> GetArticles(
        [Service] IDbContextFactory<ArticlesDbContext> factory,
        [Service] IMapper mapper)
    {
        using var context = factory.CreateDbContext();
        var entities = context.Articles.Includings();
        return mapper.Map<IEnumerable<ArticleDto>>(entities);
    }

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 20, MaxPageSize = 100)]
    [UseProjection]
    [HotChocolate.Data.UseFiltering]
    [UseSorting]
    [Authorize]
    public async Task<IEnumerable<ArticleDto>> GetBatchedArticles(
        IEnumerable<string> articleIds,
        [Service] IMapper mapper,
        ArticleBatchDataLoader dataLoader,
        CancellationToken cancellationToken)
    {
        var entities = await Task.WhenAll(articleIds.Select(id => dataLoader.LoadAsync(id, cancellationToken)));
        return mapper.Map<IEnumerable<ArticleDto>>(entities);
    }
}

public static class ArticleExtensions
{
    public static IQueryable<ArticleEntity> Includings(this IQueryable<ArticleEntity> entity)
    {
        return entity
            .Include(a => a.SimilarEntities)
            .Include(a => a.SimilarReverseEntities)
            .Include(a => a.Comments)
            .Include(a => a.Images)
            .Include(a => a.SupplierProperties);
    }
}
