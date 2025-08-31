using Microsoft.Extensions.FileProviders;

namespace API.Extensions;

public static class StaticFileExtentions
{
    public static void ConfigureStaticFiles(this IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
            RequestPath = "",
            OnPrepareResponse = context =>
            {
                var file = context.File;
                var extension = Path.GetExtension(file.Name).ToLowerInvariant();
                
                string[] blockedExtensions = [".json", ".config", ".xml", ".txt", ".log", ".env"];
                if (blockedExtensions.Contains(extension))
                {
                    context.Context.Response.StatusCode = 404;
                    context.Context.Response.ContentLength = 0;
                    context.Context.Response.Body = Stream.Null;
                }

                context.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Context.Response.Headers["X-Frame-Options"]= "DENY";
            }
        });
    }

}