using Application.DTOs;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<JwtTokenService>();
        services.Configure<PhotoOptions>(configuration.GetSection("PhotoOptions"));
        services.AddScoped<PhotoService>();
        services.AddScoped<UserService>();
        services.AddScoped<EmailService>();
        return services;
    }

}