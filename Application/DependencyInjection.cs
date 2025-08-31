using System.Reflection;
using Application.DTOs;
using Application.Services;
using Application.Validators.User;
using FluentValidation;
using FluentValidation.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;

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
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(DependencyInjection)));
        services.AddFluentValidationAutoValidation(autoValidationMvcConfiguration =>
        {
            autoValidationMvcConfiguration.ValidationStrategy = ValidationStrategy.All;
        });        return services;
    }

}