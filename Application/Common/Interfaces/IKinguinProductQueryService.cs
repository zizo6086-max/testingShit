using Application.DTOs.store;
using System.Linq.Expressions;
using Application.DTOs;
using Domain.Models.Store;

namespace Application.Common.Interfaces;

public interface IKinguinProductQueryService
{
    Task<KinguinProductListResponseDto> SearchMinimalAsync(
        int page = 1,
        int limit = 25,
        string? name = null,
        string? platform = null,
        int? regionId = null,
        string? isPreorder = null,
        string? tags = null,
        string? genre = null,
        string? updatedSince = null,
        string? sortBy = null,
        string? sortType = null,
        CancellationToken cancellationToken = default);

    Task<KinguinProductListResponseDto> SearchMinimalAsync(
        Expression<Func<KinguinProduct, bool>>? filter,
        int page = 1,
        int limit = 25,
        Func<IQueryable<KinguinProduct>, IOrderedQueryable<KinguinProduct>>? orderBy = null,
        CancellationToken cancellationToken = default);
    
    

    Task<KinguinProductDto?> GetByProductId(string productId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string productId, CancellationToken cancellationToken = default);

    public Task<KinguinProductListResponseDto> GetDeletedProducts(int page = 1, int limit = 25, CancellationToken cancellationToken = default);

    public Task<Result> RestoreAsync(string productId, CancellationToken cancellationToken = default);

}



