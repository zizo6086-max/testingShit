using Application.DTOs.store;

namespace Application.Common.Interfaces;

public interface IKinguinApiService
{
    /// <summary>
    /// Fetches products from Kinguin API for database sync
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="limit">Number of products per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response with products</returns>
    Task<KinguinProductSearchResponseDto> GetProductsForSyncAsync(
        int page = 1,
        int limit = 100,
        CancellationToken cancellationToken = default);
}
