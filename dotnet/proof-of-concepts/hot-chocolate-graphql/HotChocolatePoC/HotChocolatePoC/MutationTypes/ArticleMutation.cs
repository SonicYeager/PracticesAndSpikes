using AutoMapper;
using HotChocolate.Subscriptions;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.Types;
using Microsoft.EntityFrameworkCore;

namespace HotChocolatePoC.MutationTypes
{
    public class ArticleMutation
    {
        public async Task<ArticleDto> AddArticle(
            ArticleAddDto article, [Service] ITopicEventSender sender, [Service] IMapper mapper, [Service] DbContext context)
        {
            //le event -> Pulsar changes and what not may be possible here
            //await sender.SendAsync(nameof(ArticleAddedSubscription.BookAdded), book);
            var entity = mapper.Map<ArticleEntity>(article);
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
            return mapper.Map<ArticleDto>(entity);
        }

        [Error(typeof(KeyNotFoundException))]
        public async Task<ArticleDto> UpdateArticle(
            [ID] string articleId, ArticleDto article, [Service] IMapper mapper, [Service] DbContext context)
        {
            var existingArticle = context.Set<ArticleEntity>().Single(a => a.Id == articleId);

            var changedEntity = mapper.Map<ArticleEntity>(article);
            //apply changes
            existingArticle.Status = changedEntity.Status;
            //...

            await context.SaveChangesAsync();
            return article;
        }
    }
}