using HotChocolatePoC.Database.Context;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.QueryTypes;

namespace HotChocolatePoC.DataLoaders;

public class ArticleBatchDataLoader : BatchDataLoader<string, ArticleEntity>
{
    private readonly ArticlesDbContext _context;

    public ArticleBatchDataLoader(
        ArticlesDbContext context,
        IBatchScheduler batchScheduler,
        DataLoaderOptions options = null)
        : base(batchScheduler, options)
    {
        _context = context;
    }

    protected override async Task<IReadOnlyDictionary<string, ArticleEntity>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        // instead of fetching one person, we fetch multiple persons
        var article =  await Task
            .Run(() => _context.Articles.Includings().Where(a => keys.Any(k => k == a.Id)), cancellationToken)
            .WaitAsync(cancellationToken);
        return article.ToDictionary(x => x.Id);
    }
}