using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.ModelsConfig;

public class AppUserConfig:IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(64);
        builder.Property(u => u.RowVersion).IsRowVersion().HasConversion<byte[]>();
        builder.Property(u=> u.ImageUrl).IsRequired(false);
        var hasher = new PasswordHasher<AppUser>();
        var admin = new AppUser()
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "zyadhimself1@gmail.com",
            NormalizedEmail = "ZYADHIMSELF1@GMAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        admin.PasswordHash = hasher.HashPassword(admin, "123@Admin");
        var user = new AppUser()
        {
            Id = 2,
            UserName = "user",
            NormalizedUserName = "USER",
            Email = "zyadhimself1@gmail.com",
            NormalizedEmail = "ZYADHIMSELF1@GMAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        user.PasswordHash = hasher.HashPassword(user, "123@Admin");
        
        builder.HasData(admin,user);
        
    }
}