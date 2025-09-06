using Domain.Constants;
using Domain.Models.Auth;
using Domain.Models.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SellerApplicationConfiguration: IEntityTypeConfiguration<SellerApplication>
{
    public void Configure(EntityTypeBuilder<SellerApplication> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.PhoneNumber).IsRequired();
        builder.Property(s => s.DateOfBirth).IsRequired();
        builder.HasOne<AppUser>(s => s.User);
        builder
            .HasIndex(s => s.UserId)
            .IsUnique();
        builder.Property(s => s.DateSubmitted)
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(s => s.Status)
            .HasDefaultValue(SellerApplicationConstants.Pending);
    }
}