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
        return ProcessStatusMap
            .Any(p => !IsProcessCompleted(p.Value.Status.Status));
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
        if (!ProcessStatusMap.TryGetValue(processId, out var tuple))
            return false;

        if (IsProcessCompleted(tuple.Status.Status))
            return false;

        tuple.Cts.Cancel();
        return true;
    }

    public async Task StartBackgroundSyncAsync(string processId, string userId)
    {
        var cancellationToken = GetProcessCancellationToken(processId);
        
        try
        {
            using var scope = serviceProvider.CreateScope();
            var scopedSyncService = scope.ServiceProvider.GetRequiredService<IKinguinProductSyncService>();
            
            var result = await scopedSyncService.SyncProductsToDatabaseAsync(cancellationToken);
            
            await HandleSyncResultAsync(processId, result);
        }
        catch (OperationCanceledException)
        {
            await HandleCancellationAsync(processId);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(processId, ex);
        }
        finally
        {
            CleanupProcess(processId);
        }
    }

    private CancellationToken GetProcessCancellationToken(string processId)
    {
        return ProcessStatusMap.TryGetValue(processId, out var tuple) 
            ? tuple.Cts.Token 
            : CancellationToken.None;
    }

    private void CleanupProcess(string processId)
    {
        if (ProcessStatusMap.TryRemove(processId, out var tuple))
        {
            tuple.Cts.Dispose();
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

    private async Task HandleSyncResultAsync(string processId, KinguinSyncResult result)
    {
        var status = result.IsSuccess ? ProcessStatusConstants.Completed : ProcessStatusConstants.Failed;
        UpdateProcessStatus(processId, status, DateTime.UtcNow);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Manual database sync completed successfully. " +
                "Processed: {Total}, Created: {Created}, Updated: {Updated}, Duration: {Duration}",
                result.TotalProductsProcessed,
                result.ProductsCreated,
                result.ProductsUpdated,
                result.Duration);
        }
        else
        {
            logger.LogError("Manual database sync failed: {ErrorMessage}", result.ErrorMessage);
        }
    }

    private async Task HandleCancellationAsync(string processId)
    {
        UpdateProcessStatus(processId, ProcessStatusConstants.Cancelled, DateTime.UtcNow);
        logger.LogInformation("Database sync was cancelled for process {ProcessId}", processId);
    }

    private async Task HandleExceptionAsync(string processId, Exception ex)
    {
        UpdateProcessStatus(processId, ProcessStatusConstants.Failed, DateTime.UtcNow);
        logger.LogError(ex, "Error during background database sync for process {ProcessId}", processId);
    }

    private static bool IsProcessCompleted(string status)
    {
        return status == ProcessStatusConstants.Completed || 
               status == ProcessStatusConstants.Cancelled || 
               status == ProcessStatusConstants.Failed;
    }
}
