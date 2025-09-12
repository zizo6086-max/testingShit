using Application.Common.Interfaces;
using Application.DTOs.webhooks;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers;

[ApiController]
[Route("api/webhooks")]
public class KinguinWebhookController : ControllerBase
{
    private readonly IKinguinWebhookProcessingService _webhookService;
    private readonly ILogger<KinguinWebhookController> _logger;
    private readonly IConfiguration _configuration;

    public KinguinWebhookController(
        IKinguinWebhookProcessingService webhookService,
        ILogger<KinguinWebhookController> logger,
        IConfiguration configuration)
    {
        _webhookService = webhookService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("kinguin/product-update")]
    public async Task<IActionResult> HandleProductUpdate()
    {
        try
        {
            // Verify webhook signature
            if (!VerifyWebhookSignature())
            {
                _logger.LogWarning("Invalid webhook signature from IP: {RemoteIp}", 
                    HttpContext.Connection.RemoteIpAddress);
                return Unauthorized("Invalid signature");
            }

            // Read payload
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(payload))
            {
                _logger.LogWarning("Empty webhook payload received");
                return BadRequest("Empty payload");
            }

            // Deserialize webhook
            var webhook = JsonSerializer.Deserialize<KinguinProductUpdateWebhookDto>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (webhook == null)
            {
                _logger.LogError("Failed to deserialize webhook payload");
                return BadRequest("Invalid payload format");
            }

            _logger.LogInformation("Processing webhook - KinguinId: {KinguinId}, ProductId: {ProductId}", 
                webhook.KinguinId, webhook.ProductId);

            // Get client information for analytics
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();
            var headers = JsonSerializer.Serialize(Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

            // Process the webhook
            await _webhookService.ProcessProductUpdateAsync(webhook, clientIp, userAgent, headers);

            return NoContent(); // 204 as recommended by Kinguin
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Kinguin webhook");
            return StatusCode(500, "Internal server error");
        }
    }

    private bool VerifyWebhookSignature()
    {
        var secretKey = _configuration["Kinguin:WebhookSecret"];
        if (string.IsNullOrEmpty(secretKey))
        {
            _logger.LogWarning("Webhook secret not configured");
            return true; // Allow if no secret configured (development)
        }

        var eventSecret = Request.Headers["X-Event-Secret"].FirstOrDefault();
        var eventName = Request.Headers["X-Event-Name"].FirstOrDefault();

        return eventSecret == secretKey && eventName == "product.update";
    }
}
