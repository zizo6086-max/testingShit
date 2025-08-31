using Microsoft.Extensions.Logging;

namespace Application.Common.Extensions;

public static class LoggingExtensions
{
    public static void LogUserAction(this ILogger logger, string action, string userId, string? details = null)
    {
        logger.LogInformation("User Action: {Action} | UserId: {UserId} | Details: {Details}", 
            action, userId, details);
    }
    
    public static void LogAuthenticationAttempt(this ILogger logger, string identifier, bool success)
    {
        logger.LogInformation("Authentication Attempt: {Identifier} | Success: {Success}", 
            identifier, success);
    }
    
    public static void LogServiceOperation(this ILogger logger, string operation, string? context = null)
    {
        logger.LogInformation("Service Operation: {Operation} | Context: {Context}", 
            operation, context);
    }
}
