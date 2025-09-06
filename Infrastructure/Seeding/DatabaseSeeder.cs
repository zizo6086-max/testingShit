using Domain.Models;
using Domain.Models.Auth;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeding;

public class DatabaseSeeder
{
        public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Roles.Any())
        {
            await context.Roles.AddRangeAsync(new IdentityRole<int>
                {
                    Name = "Admin", NormalizedName = "ADMIN"
                },
                new IdentityRole<int>
                { 
                    Name = "User", NormalizedName = "USER"
                },
                new IdentityRole<int>
                {
                    Name = "Seller", NormalizedName = "SELLER"
                }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            await context.User.AddRangeAsync(new AppUser()
                {
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "zyadhimself1@gmail.com",
                    NormalizedEmail = "ZYADHIMSELF1@GMAIL.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEEwjgJslogKbvP+atYq1zBHknTif4KzsM9/UrVmaRff4X7w+bZcTovl4x9rdrlaZwQ=="
                },
                new AppUser()
                {
                    UserName = "user",
                    NormalizedUserName = "USER",
                    Email = "user@gmail.com",
                    NormalizedEmail = "USER@GMAIL.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEEwjgJslogKbvP+atYq1zBHknTif4KzsM9/UrVmaRff4X7w+bZcTovl4x9rdrlaZwQ=="
                });
            await context.SaveChangesAsync();
        }

        if (!context.UserRoles.Any())
        {
            await context.UserRoles.AddRangeAsync(new IdentityUserRole<int>()
                {
                    RoleId = 1,
                    UserId = 3
                },
                new IdentityUserRole<int>()
                {
                    RoleId = 2,
                    UserId = 4
                });
            await context.SaveChangesAsync();
        }
    }
}