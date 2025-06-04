using HotChocolatePoC.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotChocolatePoC.Database.Context;

public sealed class ArticlesDbContext : DbContext
{
    private const int MAX_LENGTH_STRING = 256;

    private const int MAX_LENGTH_LONG_STRING = 2000;

    private const int MAX_LENGTH_SHORT_STRING = 50;

    private const string COLUMN_TYPE_DECIMAL = "decimal(20, 2)";

    public ArticlesDbContext(DbContextOptions<ArticlesDbContext> options) : base(options)
    {
    }

    public DbSet<ArticleEntity> Articles => Set<ArticleEntity>();

    public DbSet<ArticleSupplierPropertiesEntity> ArticleSupplierProperties => Set<ArticleSupplierPropertiesEntity>();

    public DbSet<ArticleCommentEntity> ArticleComments => Set<ArticleCommentEntity>();

    public DbSet<ArticleSimilarityEntity> ArticleSimilarities => Set<ArticleSimilarityEntity>();

    public DbSet<ImageEntity> Images => Set<ImageEntity>();

    public DbSet<SyncLogEntity> SyncLogs => Set<SyncLogEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleEntity>(e =>
        {
            e.HasKey(a => a.Id);

            e.Property(p => p.Id).ValueGeneratedOnAdd();

            e.HasMany(a => a.SimilarEntities)
                .WithOne(s => s.Article!)
                .HasForeignKey(s => s.ArticleId);

            e.HasMany(a => a.SimilarReverseEntities)
                .WithOne(s => s.SimilarArticle!)
                .HasForeignKey(s => s.SimilarArticleId);

            e.HasMany(i => i.Images)
                .WithOne(i => i.Article)
                .HasForeignKey(i => i.ArticleId);

            e.Property(a => a.ArticleNumber)
                .HasMaxLength(MAX_LENGTH_STRING);

            e.Property(a => a.ArticleNumberSupplier)
                .HasMaxLength(MAX_LENGTH_STRING);

            e.Property(a => a.Ean)
                .HasMaxLength(MAX_LENGTH_STRING);

            e.Property(a => a.Kto)
                .HasMaxLength(MAX_LENGTH_STRING);

            e.Property(a => a.Packaging)
                .HasMaxLength(MAX_LENGTH_LONG_STRING);

            e.Property(a => a.Label1)
                .HasMaxLength(MAX_LENGTH_SHORT_STRING);

            e.Property(a => a.Label2)
                .HasMaxLength(MAX_LENGTH_SHORT_STRING);

            e.Property(a => a.Label3)
                .HasMaxLength(MAX_LENGTH_LONG_STRING);

            e.Property(a => a.Label4)
                .HasMaxLength(MAX_LENGTH_LONG_STRING);

            e.Property(a => a.ReorderNotice)
                .HasMaxLength(MAX_LENGTH_LONG_STRING);

            e.Property(a => a.PurchasePrice)
                .HasColumnType(COLUMN_TYPE_DECIMAL);

            e.Property(c => c.CreatedAt)
                .ValueGeneratedOnAdd();

            e.Property(c => c.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate();

            e.Property(p => p.CustomsTariffNumber)
                .HasMaxLength(MAX_LENGTH_SHORT_STRING);

            e.Property(p => p.CustomsTariffRate)
                .HasColumnType(COLUMN_TYPE_DECIMAL);

            e.Property(p => p.IsNew)
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<SyncLogEntity>(e =>
        {
            e.HasIndex(s => s.Type).IsUnique();
        });

        modelBuilder.Entity<ArticleSupplierPropertiesEntity>(e =>
        {
            e.HasKey(a => a.ArticleId);

            e.Property(c => c.CreatedAt)
                .ValueGeneratedOnAdd();

            e.Property(c => c.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<ArticleCommentEntity>(e =>
        {
            e.HasKey(p => p.Id);

            e.Property(m => m.CreatedAt)
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ArticleSimilarityEntity>(e =>
        {
            e.HasKey(p => new { p.ArticleId, p.SimilarArticleId });

            e.Property(m => m.CreatedAt)
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ImageEntity>(i =>
        {
            i.HasKey(e => e.Id);

            i.Property(e => e.Url)
                .HasMaxLength(MAX_LENGTH_STRING);

            i.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd();

            i.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate();
        });
    }
}