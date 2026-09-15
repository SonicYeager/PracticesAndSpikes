using AutoMapper;
using HotChocolatePoC.Database.Context;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.Types;
using Microsoft.EntityFrameworkCore;

namespace HotChocolatePoC.AutoMapperConfig;

public class CustomsTariffRateResolver  : IValueResolver<ArticleDto, ArticleEntity, string?>
{
    private readonly IDbContextFactory<ArticlesDbContext> _factory;

    public CustomsTariffRateResolver(IDbContextFactory<ArticlesDbContext> factory)
    {
        _factory = factory;
    }

    public string Resolve(ArticleDto source, ArticleEntity destination, string? destMember, ResolutionContext context)
    {
        using var dbContext = _factory.CreateDbContext();
        var s = dbContext.Set<ArticleEntity>().Where(a => a.Id == source.Id);
        return source.CustomsTariffNumber + s.First().Id;
    }
}
