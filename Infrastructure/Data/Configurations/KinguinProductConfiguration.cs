using Domain.Models.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class KinguinProductConfiguration : IEntityTypeConfiguration<KinguinProduct>
{
    public void Configure(EntityTypeBuilder<KinguinProduct> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Unique constraints
        builder.HasIndex(p => p.KinguinId)
            .IsUnique()
            .HasDatabaseName("IX_Products_KinguinId");

        builder.HasIndex(p => p.ProductId)
            .IsUnique()
            .HasDatabaseName("IX_Products_ProductId");

        // Additional indexes for common queries
        builder.HasIndex(p => p.Platform)
            .HasDatabaseName("IX_Products_Platform");

        builder.HasIndex(p => p.IsPreorder)
            .HasDatabaseName("IX_Products_IsPreorder");

        builder.HasIndex(p => p.Price)
            .HasDatabaseName("IX_Products_Price");

        builder.HasIndex(p => p.UpdatedAt)
            .HasDatabaseName("IX_Products_UpdatedAt");

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Products_IsDeleted");

        // Composite index for common filtering
        builder.HasIndex(p => new { p.IsDeleted, p.Platform, p.IsPreorder })
            .HasDatabaseName("IX_Products_Composite_Filter");    
        builder.Property(p => p.DevelopersJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.PublishersJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.GenresJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.LanguagesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.TagsJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.CountryLimitationJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.MerchantNameJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.CheapestOfferIdJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.VideosJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.SystemRequirementsJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.ImagesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.OffersJson)
            .HasColumnType("nvarchar(max)");
        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);

        // Query filters for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}