using AutoMapper;
using HotChocolate.Subscriptions;
using HotChocolatePoC.Database.Context;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.Subscriptions;
using HotChocolatePoC.Types;
using Microsoft.EntityFrameworkCore;

namespace HotChocolatePoC.MutationTypes
{
    public class ArticleMutation
    {
        public async Task<ArticleDto> AddArticle(
            ArticleAddDto article, [Service] ITopicEventSender sender, [Service] IMapper mapper, [Service] IDbContextFactory<ArticlesDbContext> factory)
        {
            var entity = mapper.Map<ArticleEntity>(article);
            entity.Id = article.ArticleNumber + article.VariantId;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await using var context = await factory.CreateDbContextAsync();
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
            var added = mapper.Map<ArticleDto>(entity);
            await sender.SendAsync(nameof(ArticleAddedSubscription.BookAdded), added);
            return added;
        }

        public async Task<ArticleDto> UpdateArticle(
            [ID] string articleId, ArticleDto article, [Service] IMapper mapper, [Service] IDbContextFactory<ArticlesDbContext> factory)
        {
            await using var context = await factory.CreateDbContextAsync();
            var existingArticle = context.Set<ArticleEntity>().SingleOrDefault(a => a.Id == articleId)
                ?? throw new System.Collections.Generic.KeyNotFoundException($"Article '{articleId}' not found.");

            var changedEntity = mapper.Map<ArticleEntity>(article);
            //apply changes
            existingArticle.Status = changedEntity.Status;
            //...

            await context.SaveChangesAsync();
            return article;
        }
    }
}
