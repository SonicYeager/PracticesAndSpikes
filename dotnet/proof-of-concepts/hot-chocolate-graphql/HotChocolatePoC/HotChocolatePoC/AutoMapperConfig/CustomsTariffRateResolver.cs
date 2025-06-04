using AutoMapper;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.Types;
using Microsoft.EntityFrameworkCore;

namespace HotChocolatePoC.AutoMapperConfig;

public class CustomsTariffRateResolver  : IValueResolver<ArticleDto, ArticleEntity, string?>
{
    private readonly DbContext _context;

    public CustomsTariffRateResolver(DbContext context)
    {
        _context = context;
    }

    public string Resolve(ArticleDto source, ArticleEntity destination, string? destMember, ResolutionContext context)
    {
        var s = _context.Set<ArticleEntity>().Where(a => a.Id == source.Id);
        return source.CustomsTariffNumber + s.First().Id;
    }
}