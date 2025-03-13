using System.Text;
using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ProductionDatabase")));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddIdentity<AppUser, IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:ValidIssuer"],
                    ValidAudience = configuration["Jwt:ValidAudience"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"] ?? 
                    throw new ApplicationException("Google ClientId is missing");
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? 
                    throw new ApplicationException("Google ClientSecret is missing");
                options.CallbackPath = configuration["Authentication:Google:CallbackPath"] ?? "/signin-google";
                options.SaveTokens = true;
            });
        return services;
    }

    public static IServiceCollection AddJwtService(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"]??
                        throw new ApplicationException("SecretKey is missing");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        
        services.AddSingleton(new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:ValidIssuer"],
            ValidAudience = configuration["Jwt:ValidAudience"],
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = key
        });
        return services;
    }
}