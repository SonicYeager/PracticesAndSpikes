using AutoMapper;
using HotChocolate.AspNetCore.Authorization;
using HotChocolatePoC.Database.Context;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.DataLoaders;
using HotChocolatePoC.Types;
using Microsoft.EntityFrameworkCore;

namespace HotChocolatePoC.QueryTypes;

public class ArticleQuery
{
    [UseProjection]
    public ArticleDto GetArticle(string id, [Service] ArticlesDbContext context, [Service] IMapper mapper)
    {
        var entity = context.Articles
            .Includings()
            .First(i => i.Id == id);
        return mapper.Map<ArticleDto>(entity);
    }

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 1000, MaxPageSize = 1000)]
    [UseProjection]
    [HotChocolate.Data.UseFiltering]
    [UseSorting]
    public IEnumerable<ArticleDto> GetArticles(
        [Service] ArticlesDbContext context,
        [Service] IMapper mapper,
        [Service] IHttpContextAccessor httpContextAccessor,
        ArticleBatchDataLoader dataLoader)
    {
        var entities = context.Articles.Includings();
        return mapper.Map<IEnumerable<ArticleDto>>(entities);
    }

    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 1000, MaxPageSize = 1000)]
    [UseProjection]
    [HotChocolate.Data.UseFiltering]
    [UseSorting]
    [Authorize]
    public IEnumerable<ArticleDto> GetBatchedArticles(
        IEnumerable<string> articleIds,
        [Service] IMapper mapper,
        [Service] IHttpContextAccessor httpContextAccessor,
        ArticleBatchDataLoader dataLoader)
    {
        var entities = dataLoader.LoadAsync(articleIds);
        return mapper.Map<IEnumerable<ArticleDto>>(entities);
    }
}

public static class ArticleExtensions
{
    public static IEnumerable<ArticleEntity> Includings(this IQueryable<ArticleEntity> entity)
    {
        return entity
            .Include(a => a.SimilarEntities)
            .Include(a => a.SimilarReverseEntities)
            .Include(a => a.Comments)
            .Include(a => a.Images)
            .Include(a => a.SupplierProperties);
    }
}