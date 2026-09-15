using HotChocolatePoC.DataLoaders;
using HotChocolatePoC.Database.Context;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.QueryTypes;
using Microsoft.EntityFrameworkCore;

namespace HotChocolatePoC.DataLoaders;

public class ArticleBatchDataLoader : BatchDataLoader<string, ArticleEntity>
{
    private readonly IDbContextFactory<ArticlesDbContext> _factory;

    public ArticleBatchDataLoader(
        IDbContextFactory<ArticlesDbContext> factory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _factory = factory;
    }

    protected override async Task<IReadOnlyDictionary<string, ArticleEntity>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        // instead of fetching one article, we fetch multiple articles in one roundtrip
        await using var context = await _factory.CreateDbContextAsync(cancellationToken);
        return await context.Articles.Includings()
            .Where(a => keys.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);
    }
}
