using HotChocolatePoC.Database.Context;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace HotChocolatePoC.E2ETests;

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Name, "e2e"));
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, "Test")));
    }
}

/// <summary>
/// Boots the real API with SQLite instead of MySQL. One instance per test for isolation.
/// </summary>
public sealed class GraphQlTestFactory : WebApplicationFactory<Program>
{
    private readonly bool _useTestAuth;
    private SqliteConnection? _connection;

    public GraphQlTestFactory(bool useTestAuth = false)
    {
        _useTestAuth = useTestAuth;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Drop every EntityFramework registration from Program.cs (MySQL pooled
            // factory plus its supporting descriptors). EF allows only a single
            // database provider per service provider, so leftovers of the MySQL
            // registration collide with the SQLite one below.
            var efDescriptors = services
                .Where(d => d.ServiceType.FullName?.Contains("EntityFramework") == true
                    || (d.ImplementationType?.FullName?.Contains("EntityFramework") == true))
                .ToList();
            foreach (var descriptor in efDescriptors)
                services.Remove(descriptor);
            services.AddDbContextFactory<ArticlesDbContext>(options =>
                options.UseSqlite(_connection));
            if (_useTestAuth)
            {
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            }
        });
    }

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        using var scope = Services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ArticlesDbContext>>();
        await using var db = await factory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
        Seed(db);
        await db.SaveChangesAsync();
    }

    public async Task CleanupAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }

    private static void Seed(ArticlesDbContext db)
    {
        var stamp = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var article1 = new ArticleEntity
        {
            Id = "A-1001",
            ArticleNumber = "A-100",
            VariantId = 1,
            Status = HotChocolatePoC.Database.Entities.ArticleState.Active,
            PurchasePrice = 10.5m,
            IsNew = true,
            CreatedAt = stamp,
            UpdatedAt = stamp,
            SupplierProperties = new ArticleSupplierPropertiesEntity
            {
                ArticleId = "A-1001",
                ItemWeight = 1.5,
                CreatedAt = stamp,
                UpdatedAt = stamp,
            },
        };
        article1.Comments.Add(new ArticleCommentEntity
        {
            ArticleId = "A-1001",
            Content = "Looks good",
            CreatedById = 7,
            CreatedAt = stamp,
        });
        article1.Images.Add(new ImageEntity
        {
            ArticleId = "A-1001",
            Url = "https://img/a1.png",
            CreatedAt = stamp,
            UpdatedAt = stamp,
        });
        var article2 = new ArticleEntity
        {
            Id = "B-2002",
            ArticleNumber = "B-200",
            VariantId = 2,
            Status = HotChocolatePoC.Database.Entities.ArticleState.Inactive,
            PurchasePrice = 20.0m,
            IsNew = false,
            CreatedAt = stamp,
            UpdatedAt = stamp,
        };
        db.Articles.Add(article1);
        db.Articles.Add(article2);
    }

    public static async Task<JsonDocument> PostGraphQlAsync(HttpClient client, string query)
    {
        var payload = JsonSerializer.Serialize(new { query });
        using var response = await client.PostAsync(
            "/graphql", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"GraphQL HTTP {(int)response.StatusCode}: {body.Substring(0, Math.Min(500, body.Length))}");
        return JsonDocument.Parse(body);
    }
}
