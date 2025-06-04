using HotChocolatePoC.Types;

namespace HotChocolatePoC.Subscriptions;

public class ArticleAddedSubscription
{
    [Subscribe]
    public ArticleDto BookAdded([EventMessage] ArticleDto articleDto) => articleDto;
}