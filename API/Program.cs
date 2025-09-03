using Application;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Filters;
using API.Extensions;
using API.Filters;
using API.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // Optional
            });  
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()     
                    .AllowAnyHeader();
            });
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddJwtService(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var app = builder.Build();

        app.UseGlobalExceptionHandling();
        
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<PathTraversalProtectionMiddleware>();
        app.UseHttpsRedirection();
        app.ConfigureStaticFiles(app.Environment);
        app.UseRouting();
        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "openapi/{documentName}.json";
        });
        app.MapScalarApiReference(options =>
        {
            options.WithLayout(ScalarLayout.Modern);
            options.WithTitle("UserZone-api")
                .WithTheme(ScalarTheme.DeepSpace)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
        app.MapControllers().RequireCors("AllowAll");

        app.Run();
    }
}