using Application.DTOs;

namespace Application.Common.Interfaces;

public interface IKinguinProductSyncService
{
    /// <summary>
    /// Syncs Kinguin products to database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result with statistics</returns>
    Task<KinguinSyncResult> SyncProductsToDatabaseAsync(CancellationToken cancellationToken = default);
}
