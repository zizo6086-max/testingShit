using Application.Common.Interfaces;
using Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class KinguinProductBackgroundSyncService(
    IServiceProvider serviceProvider,
    ILogger<KinguinProductBackgroundSyncService> logger)
    : BackgroundService
{
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kinguin Product Background Sync Service started");

        // Perform initial sync on startup
        await PerformDatabaseSyncAsync(stoppingToken);

        // Set up periodic sync
        using var timer = new PeriodicTimer(_syncInterval);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                await PerformDatabaseSyncAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Kinguin Product Background Sync Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Kinguin Product Background Sync Service");
                
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        logger.LogInformation("Kinguin Product Background Sync Service stopped");
    }

    private async Task PerformDatabaseSyncAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Kinguin products database sync");

        try
        {
            using var scope = serviceProvider.CreateScope();
            var syncService = scope.ServiceProvider.GetRequiredService<IKinguinProductSyncService>();
            
            var result = await syncService.SyncProductsToDatabaseAsync(cancellationToken);
            
            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Database sync completed. " +
                    "Processed: {Total}, Created: {Created}, Updated: {Updated}, Failed: {Failed}, Duration: {Duration}",
                    result.TotalProductsProcessed,
                    result.ProductsCreated,
                    result.ProductsUpdated,
                    result.ProductsFailed,
                    result.Duration);
            }
            else
            {
                logger.LogError("Database sync failed: {ErrorMessage}", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to perform database sync of Kinguin products");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Kinguin Product Background Sync Service is stopping...");
        await base.StopAsync(cancellationToken);
    }
}
