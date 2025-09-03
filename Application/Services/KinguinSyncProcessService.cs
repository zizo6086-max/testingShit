using System.Collections.Concurrent;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class KinguinSyncProcessService(
    IServiceProvider serviceProvider,
    ILogger<KinguinSyncProcessService> logger)
    : IKinguinSyncProcessService
{
    private static readonly ConcurrentDictionary<string, (ProcessStatus Status, CancellationTokenSource Cts)> 
        ProcessStatusMap = new();

    public bool IsAnyProcessRunning()
    {
        return ProcessStatusMap.Any(p => p.Value.Status.Status == ProcessStatusConstants.Running);
    }

    public string StartSyncProcess()
    {
        var processId = Guid.NewGuid().ToString();
        var cts = new CancellationTokenSource();
        
        var status = new ProcessStatus
        {
            Id = processId,
            Status = ProcessStatusConstants.Running,
            StartedAt = DateTime.UtcNow
        };
        
        ProcessStatusMap[processId] = (status, cts);
        return processId;
    }

    public ProcessStatus? GetProcessStatus(string processId)
    {
        return ProcessStatusMap.TryGetValue(processId, out var tuple) ? tuple.Status : null;
    }

    public bool CancelProcess(string processId)
    {
        if (!ProcessStatusMap.TryGetValue(processId, out var tuple) || 
            tuple.Status.Status != ProcessStatusConstants.Running)
            return false;

        tuple.Cts.Cancel();
        return true;
    }

    public async Task StartBackgroundSyncAsync(string processId, string userId)
    {
        var cancellationToken = ProcessStatusMap.TryGetValue(processId, out var tuple) 
            ? tuple.Cts.Token 
            : CancellationToken.None;
        
        try
        {
            using var scope = serviceProvider.CreateScope();
            var scopedSyncService = scope.ServiceProvider.GetRequiredService<IKinguinProductSyncService>();
            
            var result = await scopedSyncService.SyncProductsToDatabaseAsync(cancellationToken);
            
            // Update status and log result
            var status = result.IsSuccess ? ProcessStatusConstants.Completed : ProcessStatusConstants.Failed;
            UpdateProcessStatus(processId, status, DateTime.UtcNow);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Manual database sync completed successfully. " +
                    "Processed: {Total}, Created: {Created}, Updated: {Updated}, Duration: {Duration}",
                    result.TotalProductsProcessed, result.ProductsCreated, result.ProductsUpdated, result.Duration);
            }
            else
            {
                logger.LogError("Manual database sync failed: {ErrorMessage}", result.ErrorMessage);
            }
        }
        catch (OperationCanceledException)
        {
            UpdateProcessStatus(processId, ProcessStatusConstants.Cancelled, DateTime.UtcNow);
            logger.LogInformation("Database sync was cancelled for process {ProcessId}", processId);
        }
        catch (Exception ex)
        {
            UpdateProcessStatus(processId, ProcessStatusConstants.Failed, DateTime.UtcNow);
            logger.LogError(ex, "Error during background database sync for process {ProcessId}", processId);
        }
        finally
        {
            // Cleanup process
            if (ProcessStatusMap.TryRemove(processId, out var cleanupTuple))
            {
                cleanupTuple.Cts.Dispose();
            }
        }
    }

    private void UpdateProcessStatus(string processId, string status, DateTime? completedAt = null)
    {
        if (ProcessStatusMap.TryGetValue(processId, out var tuple))
        {
            tuple.Status.Status = status;
            if (completedAt.HasValue)
            {
                tuple.Status.CompletedAt = completedAt.Value;
            }
            ProcessStatusMap[processId] = tuple;
        }
    }
}