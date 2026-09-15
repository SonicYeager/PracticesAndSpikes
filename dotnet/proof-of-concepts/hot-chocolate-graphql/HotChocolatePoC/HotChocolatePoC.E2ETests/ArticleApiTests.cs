using System.Text.Json;
using Xunit;

namespace HotChocolatePoC.E2ETests;

public sealed class ArticleApiTests
{
    private static async Task<GraphQlTestFactory> CreateFactoryAsync(bool auth = false)
    {
        var factory = new GraphQlTestFactory(auth);
        await factory.InitializeAsync();
        return factory;
    }

    private static string Data(JsonDocument doc, params string[] path)
    {
        var current = doc.RootElement.GetProperty("data");
        foreach (var segment in path)
            current = current.GetProperty(segment);
        return current.ToString();
    }

    [Fact]
    public async Task Articles_ReturnsSeededRowsWithTotalCount()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ articles(first: 10) { totalCount edges { node { id articleNumber } } } }");

            Assert.Equal("2", Data(doc, "articles", "totalCount"));
            Assert.Contains("A-100", doc.RootElement.ToString());
            Assert.Contains("B-200", doc.RootElement.ToString());
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task Articles_FilterByArticleNumber_ReturnsSingleRow()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ articles(first: 10, where: { articleNumber: { eq: \"A-100\" } }) { totalCount edges { node { id } } } }");

            Assert.Equal("1", Data(doc, "articles", "totalCount"));
            Assert.Contains("A-1001", doc.RootElement.ToString());
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task Articles_OrderByPriceDesc_ReturnsMostExpensiveFirst()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ articles(order: { purchasePrice: DESC }, first: 2) { edges { node { articleNumber purchasePrice } } } }");

            var edges = doc.RootElement.GetProperty("data").GetProperty("articles").GetProperty("edges");
            Assert.Equal("B-200", edges[0].GetProperty("node").GetProperty("articleNumber").GetString());
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task Article_ById_ReturnsRowWithNestedRelations()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ article(id: \"A-1001\") { id articleNumber comments { content } supplierProperties { itemWeight } } }");

            var article = doc.RootElement.GetProperty("data").GetProperty("article");
            Assert.Equal("A-100", article.GetProperty("articleNumber").GetString());
            Assert.Equal("Looks good", article.GetProperty("comments")[0].GetProperty("content").GetString());
            Assert.Equal(1.5, article.GetProperty("supplierProperties").GetProperty("itemWeight").GetDouble());
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task Article_UnknownId_ReturnsTypedUserError()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ article(id: \"NOPE\") { id } }");

            var root = doc.RootElement;
            Assert.True(root.GetProperty("data").ValueKind == JsonValueKind.Null);
            var errors = root.GetProperty("errors");
            Assert.True(errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0);
            Assert.Equal("KEY_NOT_FOUND", errors[0].GetProperty("extensions").GetProperty("code").GetString());
            Assert.Contains("NOPE", errors[0].GetProperty("message").GetString());
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task AddArticle_CreatesWithServerGeneratedIdAndPersists()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var client = factory.CreateClient();
            var doc = await GraphQlTestFactory.PostGraphQlAsync(client,
                "mutation { addArticle(input: { article: { articleNumber: \"C-300\", status: ACTIVE, articleNumberSupplier: \"SUP-1\", ean: \"123\", kto: \"K1\", articleMatchcode: \"match\", minimumOrderQuantity: 5, packagingUnit: 10, packaging: \"box\", purchasePrice: 30.5, variantId: 3, variantValue: \"red\", variantType: \"color\", volume: 1, length: 2, width: 3, height: 4, outerBoxVolume: 5, outerBoxLength: 6, outerBoxWidth: 7, outerBoxHeight: 8, label1: \"l1\", label2: \"l2\", label3: \"l3\", label4: \"l4\", reorderNotice: \"none\", isNew: true } }) { articleDto { id articleNumber } } }");

            Assert.Equal("C-3003", Data(doc, "addArticle", "articleDto", "id"));

            var reread = await GraphQlTestFactory.PostGraphQlAsync(
                client, "{ article(id: \"C-3003\") { id articleNumber } }");
            Assert.Equal("C-300", Data(reread, "article", "articleNumber"));
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task UpdateArticle_UnknownId_ReturnsTypedUserError()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "mutation { updateArticle(input: { articleId: \"NOPE\", article: { id: \"NOPE\", articleNumber: \"X\", status: ACTIVE, articleNumberSupplier: \"s\", ean: \"e\", kto: \"k\", articleMatchcode: \"m\", minimumOrderQuantity: 1, packagingUnit: 1, packaging: \"p\", purchasePrice: 1.5, variantId: 1, variantValue: \"v\", variantType: \"t\", volume: 1, length: 1, width: 1, height: 1, outerBoxVolume: 1, outerBoxLength: 1, outerBoxWidth: 1, outerBoxHeight: 1, label1: \"a\", label2: \"b\", label3: \"c\", label4: \"d\", reorderNotice: \"r\", isNew: false, comments: [], similarArticles: [] } }) { articleDto { id } } }");

            var root = doc.RootElement;
            var errors = root.GetProperty("errors");
            Assert.True(errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0);
            Assert.Equal("KEY_NOT_FOUND", errors[0].GetProperty("extensions").GetProperty("code").GetString());
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task BatchedArticles_WithoutAuth_IsRejected()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ batchedArticles(first: 10, articleIds: [\"A-1001\"]) { edges { node { id } } } }");

            var root = doc.RootElement;
            Assert.True(root.GetProperty("errors").ValueKind == JsonValueKind.Array
                && root.GetProperty("errors").GetArrayLength() > 0);
            Assert.True(root.GetProperty("data").GetProperty("batchedArticles").ValueKind == JsonValueKind.Null);
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task BatchedArticles_WithTestAuth_ReturnsBatchedRows()
    {
        await using var factory = await CreateFactoryAsync(auth: true);
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ batchedArticles(first: 10, articleIds: [\"A-1001\", \"B-2002\"]) { totalCount edges { node { id articleNumber } } } }");

            Assert.Equal("2", Data(doc, "batchedArticles", "totalCount"));
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }

    [Fact]
    public async Task Schema_ExposesExpectedQueryMutationSubscriptionFields()
    {
        await using var factory = await CreateFactoryAsync();
        try
        {
            var doc = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ __schema { queryType { fields { name } } mutationType { fields { name } } subscriptionType { fields { name } } } }");

            static string[] FieldNames(JsonDocument doc, string type)
            {
                var names = new List<string>();
                foreach (var field in doc.RootElement.GetProperty("data").GetProperty("__schema")
                    .GetProperty(type).GetProperty("fields").EnumerateArray())
                    names.Add(field.GetProperty("name").GetString()!);
                return names.ToArray();
            }

            Assert.Contains("articles", FieldNames(doc, "queryType"));
            Assert.Contains("article", FieldNames(doc, "queryType"));
            Assert.Contains("batchedArticles", FieldNames(doc, "queryType"));
            Assert.Contains("addArticle", FieldNames(doc, "mutationType"));
            Assert.Contains("updateArticle", FieldNames(doc, "mutationType"));
            Assert.Contains("bookAdded", FieldNames(doc, "subscriptionType"));

            var payload = await GraphQlTestFactory.PostGraphQlAsync(
                factory.CreateClient(),
                "{ __type(name: \"AddArticlePayload\") { fields { name } } }");
            var payloadFields = payload.RootElement.GetProperty("data").GetProperty("__type")
                .GetProperty("fields").EnumerateArray()
                .Select(f => f.GetProperty("name").GetString()).ToArray();
            Assert.Contains("articleDto", payloadFields);
        }
        finally
        {
            await factory.CleanupAsync();
        }
    }
}
