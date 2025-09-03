using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.store;
using Application.Mappers;
using Domain.Models.Store;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class KinguinProductSyncService(
    IKinguinApiService kinguinApiService,
    AppDbContext context,
    ILogger<KinguinProductSyncService> logger)
    : IKinguinProductSyncService
{
    private readonly int _batchSize = 100;

    public async Task<KinguinSyncResult> SyncProductsToDatabaseAsync(CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var result = new KinguinSyncResult
        {
            StartTime = startTime,
            IsSuccess = true
        };

        try
        {
            int currentPage = 1;
            int totalProcessed = 0;
            int created = 0;
            int updated = 0;
            int failed = 0;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    var searchResponse = await kinguinApiService.GetProductsForSyncAsync(
                        page: currentPage,
                        limit: _batchSize,
                        cancellationToken: cancellationToken);

                    if (searchResponse.Results.Count == 0)
                    {
                        break;
                    }

                    var batchResult = await ProcessProductBatchAsync(searchResponse.Results, cancellationToken);
                    
                    totalProcessed += batchResult.TotalProcessed;
                    created += batchResult.Created;
                    updated += batchResult.Updated;
                    failed += batchResult.Failed;

                    result.TotalPagesProcessed = currentPage;

                    if (searchResponse.Results.Count < _batchSize)
                    {
                        break;
                    }

                    currentPage++;
                    await Task.Delay(100, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing page {Page}", currentPage);
                    failed += _batchSize;
                    currentPage++;
                    
                    if (currentPage > 100)
                    {
                        break;
                    }
                }
            }

            result.EndTime = DateTime.UtcNow;
            result.TotalProductsProcessed = totalProcessed;
            result.ProductsCreated = created;
            result.ProductsUpdated = updated;
            result.ProductsFailed = failed;

            return result;
        }
        catch (Exception ex)
        {
            var endTime = DateTime.UtcNow;
            logger.LogError(ex, "Failed to sync products to database");
            
            return KinguinSyncResult.CreateFailure(
                startTime,
                endTime,
                "Database sync failed",
                new List<string> { ex.Message });
        }
    }

    private async Task<BatchProcessResult> ProcessProductBatchAsync(
        List<KinguinProductDto> products,
        CancellationToken cancellationToken)
    {
        var result = new BatchProcessResult();
        var productsToUpsert = new List<KinguinProduct>();

        var kinguinIds = products.Select(p => p.KinguinId).ToList();
        var existingProducts = await context.KinguinProducts
            .Where(p => kinguinIds.Contains(p.KinguinId))
            .ToDictionaryAsync(p => p.KinguinId, p => p, cancellationToken);

        foreach (var productDto in products)
        {
            try
            {
                result.TotalProcessed++;

                var existingProduct = existingProducts.GetValueOrDefault(productDto.KinguinId);
                
                if (existingProduct != null)
                {
                    var updatedProduct = productDto.MapToEntity(.10, existingProduct);
                    productsToUpsert.Add(updatedProduct);
                    result.Updated++;
                }
                else
                {
                    var newProduct = productDto.MapToEntity(.10);
                    productsToUpsert.Add(newProduct);
                    result.Created++;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing product {KinguinId}", productDto.KinguinId);
                result.Failed++;
            }
        }

        if (productsToUpsert.Count != 0)
        {
            try
            {
                foreach (var product in productsToUpsert)
                {
                    if (product.Id == 0)
                    {
                        context.KinguinProducts.Add(product);
                    }
                    else
                    {
                        context.KinguinProducts.Update(product);
                    }
                }

                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save products to database");
                result.Failed += productsToUpsert.Count;
                result.Created = 0;
                result.Updated = 0;
            }
        }

        return result;
    }

    private class BatchProcessResult
    {
        public int TotalProcessed { get; set; }
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Failed { get; set; }
    }
}
