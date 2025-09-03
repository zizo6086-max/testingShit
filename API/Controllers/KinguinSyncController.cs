using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KinguinSyncController(
    IKinguinSyncProcessService syncProcessService,
    ILogger<KinguinSyncController> logger)
    : ControllerBase
{
    /// <summary>
    /// Manually trigger database synchronization of Kinguin products
    /// </summary>
    /// <returns>Synchronization result with statistics</returns>
    [HttpPost("sync-database")]
    [Authorize(Roles = "Admin")]
    public ActionResult SyncDatabase()
    {
        try
        {
            var userId = User.Identity?.Name ?? "Unknown";
            logger.LogInformation("Manual database sync triggered by user {UserId}", userId);
            
            var processId = syncProcessService.StartSyncProcess();
            
            // Start sync in background
            _ = Task.Run(async () => await syncProcessService.StartBackgroundSyncAsync(processId, userId));

            return Accepted(new
            {
                Id = processId,
                StatusUrl = $"/api/kinguinsync/status/{processId}",
                CancelUrl = $"/api/kinguinsync/cancel/{processId}"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting manual database sync");
            return StatusCode(500, new { error = "Internal server error starting database synchronization" });
        }
    }

    [HttpGet("status/{processId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetStatus(string processId)
    {
        if (string.IsNullOrEmpty(processId))
        {
            return BadRequest(new { Message = "Process ID is required" });
        }

        var status = syncProcessService.GetProcessStatus(processId);
        if (status == null)
        {
            return NotFound(new { Message = "Process not found" });
        }

        return Ok(new
        {
            status.Id,
            status.Status,
            status.StartedAt,
            status.CompletedAt,
            Duration = CalculateDuration(status.StartedAt, status.CompletedAt)
        });
    }
    [HttpPost("cancel/{processId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult CancelProcess(string processId)
    {
        if (string.IsNullOrEmpty(processId))
        {
            return BadRequest(new { Message = "Process ID is required" });
        }

        var status = syncProcessService.GetProcessStatus(processId);
        if (status == null)
        {
            return NotFound(new { Message = "Process not found" });
        }

        if (IsProcessCompleted(status.Status))
        {
            return BadRequest(new { Message = $"Process cannot be cancelled as it is already {status.Status.ToLower()}" });
        }

        var cancelled = syncProcessService.CancelProcess(processId);
        if (!cancelled)
        {
            return BadRequest(new { Message = "Unable to cancel process" });
        }

        return Ok(new { Message = "Cancellation requested", ProcessId = processId });
    }

    private static string CalculateDuration(DateTime startedAt, DateTime? completedAt)
    {
        var endTime = completedAt ?? DateTime.UtcNow;
        return (endTime - startedAt).ToString(@"hh\:mm\:ss");
    }

    private static bool IsProcessCompleted(string status)
    {
        return status == ProcessStatusConstants.Completed || 
               status == ProcessStatusConstants.Cancelled || 
               status == ProcessStatusConstants.Failed;
    }
}
