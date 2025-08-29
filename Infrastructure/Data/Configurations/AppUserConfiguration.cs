using Domain.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(64);
        builder.Property(u => u.RowVersion).IsRowVersion().HasConversion<byte[]>();
        builder.Property(u=> u.ImageUrl).IsRequired(false);
    }
}
