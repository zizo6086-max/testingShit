using Domain.Models;
using Domain.Models.Auth;
using Domain.Models.Store;
using Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<int>, int>(options)
{
    public DbSet<AppUser> User { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<KinguinProduct> KinguinProducts { get; set; }
    public DbSet<StoreConfigs> StoreConfigs { get; set; }
    public DbSet<SellerApplication> SellerApplications { get; set; }
    public DbSet<WebhookEvent> WebhookEvents { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppUserConfiguration).Assembly);
    }
}