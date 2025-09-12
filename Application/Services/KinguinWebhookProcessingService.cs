using Application.Common.Interfaces;
using Application.DTOs.webhooks;
using Application.Mappers;
using Domain.Models.Store;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Services;

public interface IKinguinWebhookProcessingService
{
    Task ProcessProductUpdateAsync(KinguinProductUpdateWebhookDto webhook, string? clientIp = null, string? userAgent = null, string? headers = null);
}

public class KinguinWebhookProcessingService(
    AppDbContext context,
    IKinguinApiService apiService,
    ILogger<KinguinWebhookProcessingService> logger)
    : IKinguinWebhookProcessingService
{
    private readonly IKinguinApiService _apiService = apiService;

    public async Task ProcessProductUpdateAsync(KinguinProductUpdateWebhookDto webhook, string? clientIp = null, string? userAgent = null, string? headers = null)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        WebhookEvent? webhookEvent = null;

        try
        {
            // Create webhook event record for analytics
            webhookEvent = new WebhookEvent
            {
                EventType = "product.update",
                Source = "Kinguin",
                ProductId = webhook.ProductId,
                KinguinId = webhook.KinguinId,
                Payload = JsonSerializer.Serialize(webhook),
                Headers = headers,
                ClientIp = clientIp,
                UserAgent = userAgent,
                Status = WebhookEventStatus.Processing,
                ReceivedAt = DateTime.UtcNow
            };

            context.WebhookEvents.Add(webhookEvent);
            await context.SaveChangesAsync();

            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var existingProduct = await context.KinguinProducts
                    .FirstOrDefaultAsync(p => 
                        (p.KinguinId == webhook.KinguinId || p.ProductId == webhook.ProductId) && 
                        !p.IsDeleted);

                if (existingProduct == null)
                {
                    await CreateProductFromWebhookAsync(webhook);
                }
                else
                {
                    await UpdateProductFromWebhookAsync(existingProduct, webhook);
                }

                await transaction.CommitAsync();

                // Update webhook event as successful
                stopwatch.Stop();
                webhookEvent.Status = WebhookEventStatus.Success;
                webhookEvent.ProcessedAt = DateTime.UtcNow;
                webhookEvent.ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds;
                await context.SaveChangesAsync();

                logger.LogInformation("Successfully processed webhook for KinguinId: {KinguinId} in {ProcessingTime}ms", 
                    webhook.KinguinId, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Update webhook event as failed
            if (webhookEvent != null)
            {
                webhookEvent.Status = WebhookEventStatus.Failed;
                webhookEvent.ErrorMessage = ex.Message;
                webhookEvent.ProcessedAt = DateTime.UtcNow;
                webhookEvent.ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds;
                await context.SaveChangesAsync();
            }

            logger.LogError(ex, "Failed to process webhook for KinguinId: {KinguinId} in {ProcessingTime}ms", 
                webhook.KinguinId, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task CreateProductFromWebhookAsync(KinguinProductUpdateWebhookDto webhook)
    {
        // Get store configuration (same as existing sync service)
        var storeConfig = await context.StoreConfigs.AsNoTracking().FirstOrDefaultAsync();
        double priceMargin = storeConfig?.PriceMargin ?? 0.10;

        var newProduct = new KinguinProduct
        {
            KinguinId = webhook.KinguinId,
            ProductId = webhook.ProductId,
            Name = $"Product {webhook.KinguinId}", // Placeholder - daily sync will update
            Qty = webhook.Qty,
            TextQty = webhook.TextQty,
            TotalQty = webhook.Qty,
            Price = 0, // Will be set by daily sync
            CheapestOfferIdJson = webhook.CheapestOfferId?.Any() == true 
                ? JsonSerializer.Serialize(webhook.CheapestOfferId) 
                : null,
            UpdatedAt = webhook.UpdatedAt,
            LastApiUpdate = null, // Force full refresh by daily sync
            CreatedAt = DateTime.UtcNow
        };

        context.KinguinProducts.Add(newProduct);
        await context.SaveChangesAsync();

        logger.LogInformation("Created new product from webhook - KinguinId: {KinguinId}", webhook.KinguinId);
    }

    private async Task UpdateProductFromWebhookAsync(KinguinProduct existingProduct, KinguinProductUpdateWebhookDto webhook)
    {
        // Detect if significant changes require full refresh
        var needsFullRefresh = DetectSignificantChanges(existingProduct, webhook);

        // Always update quantities and basic fields from webhook
        existingProduct.Qty = webhook.Qty;
        existingProduct.TextQty = webhook.TextQty;
        existingProduct.UpdatedAt = webhook.UpdatedAt;
        existingProduct.CheapestOfferIdJson = webhook.CheapestOfferId?.Any() == true 
            ? JsonSerializer.Serialize(webhook.CheapestOfferId)
            : null;

        if (needsFullRefresh)
        {
            // Reset LastApiUpdate to trigger full refresh by daily sync
            existingProduct.LastApiUpdate = null;
            logger.LogInformation("Marked product for full refresh - KinguinId: {KinguinId}", webhook.KinguinId);
        }
        else
        {
            existingProduct.LastApiUpdate = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }

    private bool DetectSignificantChanges(KinguinProduct product, KinguinProductUpdateWebhookDto webhook)
    {
        // Stock status changed
        var wasOutOfStock = product.Qty == 0 && product.TextQty == 0;
        var nowHasStock = webhook.Qty > 0 || webhook.TextQty > 0;
        var nowOutOfStock = webhook.Qty == 0 && webhook.TextQty == 0;
        var hadStock = product.Qty > 0 || product.TextQty > 0;

        var stockStatusChanged = (wasOutOfStock && nowHasStock) || (hadStock && nowOutOfStock);

        // Cheapest offer changed (might indicate price change)
        var currentCheapestOffers = string.IsNullOrEmpty(product.CheapestOfferIdJson) 
            ? []
            : JsonSerializer.Deserialize<List<string>>(product.CheapestOfferIdJson) ?? [];
        
        var offersChanged = !webhook.CheapestOfferId.SequenceEqual(currentCheapestOffers);

        return stockStatusChanged || offersChanged;
    }
}
