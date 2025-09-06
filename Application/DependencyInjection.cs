using System.Reflection;
using Application.Common.Interfaces;
using Application.DTOs;
using Application.Services;
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
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.Configure<PhotoOptions>(configuration.GetSection("PhotoOptions"));
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmailService, EmailService>();
        
        // Kinguin API services
        services.AddHttpClient<IKinguinApiService, KinguinApiService>();
        services.AddScoped<IKinguinProductSyncService, KinguinProductSyncService>();
        services.AddScoped<IKinguinProductQueryService, KinguinProductQueryService>();
        services.AddScoped<IStoreConfigService, StoreConfigService>();
        services.AddScoped<IRoleService, RoleService>();
        //services.AddHostedService<KinguinProductBackgroundSyncService>(); stoped for now
        
        // Process management services
        services.AddSingleton<IKinguinSyncProcessService, KinguinSyncProcessService>();
        
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(DependencyInjection)));
        services.AddFluentValidationAutoValidation(autoValidationMvcConfiguration =>
        {
            autoValidationMvcConfiguration.ValidationStrategy = ValidationStrategy.All;
        });
        
        return services;
    }

}