using System.Web;

namespace API.Middleware;

public class PathTraversalProtectionMiddleware(RequestDelegate next, ILogger<PathTraversalProtectionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var decodedPath = HttpUtility.UrlDecode(path);
        
        // Check for path traversal patterns
        string[] dangerousPatterns = 
        {
            "..", 
            "~", 
            "appsettings", 
            ".config", 
            ".env",
            "web.config",
            ".cs",
            ".dll",
            ".exe"
        };

        foreach (var pattern in dangerousPatterns)
        {
            if (decodedPath.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Potential path traversal attack detected from {RemoteIp}: {Path}", 
                    context.Connection.RemoteIpAddress, path);
                    
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access denied");
                return;
            }
        }

        await next(context);
    }
}