using AutoMapper;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.Types;

namespace HotChocolatePoC.TypeExtensions;

[ExtendObjectType(typeof(ArticleDto))]
public class ArticleExtension
{
    public string GetArticleEntity([Parent] ArticleDto articleDto, [Service] IMapper mapper)
    {
        return mapper.Map<ArticleEntity>(articleDto).CustomsTariffNumber;
    }

    //[BindMember(nameof(Book.Author))]
    //public Author GetAuthor([Parent] Book articleDto)
    //{
    //    // Omitted code for brevity
    //    return new Author(2, "More!", "Mooore!");
    //}
}