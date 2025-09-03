using Application.Common.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KinguinSyncController(
    IKinguinProductSyncService syncService,
    ILogger<KinguinSyncController> logger)
    : ControllerBase
{
    /// <summary>
    /// Manually trigger database synchronization of Kinguin products
    /// </summary>
    /// <returns>Synchronization result with statistics</returns>
    [HttpPost("sync-database")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<KinguinSyncResult>> SyncDatabase(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Manual database sync triggered by user {UserId}", User.Identity?.Name);
            
            var result = await syncService.SyncProductsToDatabaseAsync(cancellationToken);
            
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

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during manual database sync");
            return StatusCode(500, new { error = "Internal server error during database synchronization" });
        }
    }
}
