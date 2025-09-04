using Application.Common.Interfaces;
using Application.DTOs.store;
using Application.Mappers;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Application.DTOs;
using Domain.Models.Store;

namespace Application.Services;

public class KinguinProductQueryService(AppDbContext context) : IKinguinProductQueryService
{
    private static IQueryable<KinguinProduct> ApplyOrdering(IQueryable<KinguinProduct> query, string? sortBy, string? sortType)
    {
        var sortDescending = string.Equals(sortType, "desc", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(sortType);
        switch (sortBy?.ToLower())
        {
            case "price":
                return sortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
            case "name":
                return sortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
            default:
                return query.OrderByDescending(p => p.UpdatedAt);
        }
    }

    private static IQueryable<KinguinProduct> ApplyFilters(IQueryable<KinguinProduct> query,
        string? name,
        string? platform,
        int? regionId,
        string? isPreorder,
        string? tags,
        string? genre,
        string? updatedSince)
    {
        if (!string.IsNullOrWhiteSpace(name) && name.Length >= 3)
        {
            var lowered = name.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowered) || (p.OriginalName != null && p.OriginalName.ToLower().Contains(lowered)));
        }

        if (!string.IsNullOrWhiteSpace(platform))
        {
            query = query.Where(p => p.Platform != null && p.Platform.Contains(platform));
        }

        if (regionId.HasValue)
        {
            query = query.Where(p => p.RegionId == regionId);
        }

        if (!string.IsNullOrWhiteSpace(isPreorder))
        {
            if (isPreorder.Equals("yes", StringComparison.OrdinalIgnoreCase))
                query = query.Where(p => p.IsPreorder);
            else if (isPreorder.Equals("no", StringComparison.OrdinalIgnoreCase))
                query = query.Where(p => !p.IsPreorder);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.ToLower())
                .ToList();
            foreach (var tag in tagList)
            {
                query = query.Where(p => p.TagsJson != null && p.TagsJson.ToLower().Contains(tag));
            }
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            var genreList = genre.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(g => g.ToLower())
                .ToList();
            foreach (var g in genreList)
            {
                query = query.Where(p => p.GenresJson != null && p.GenresJson.ToLower().Contains(g));
            }
        }

        if (!string.IsNullOrWhiteSpace(updatedSince))
        {
            if (DateTime.TryParse(updatedSince, out var since))
            {
                query = query.Where(p => (p.UpdatedAt ?? p.CreatedAt) >= since);
            }
        }

        return query;
    }

    public async Task<KinguinProductListResponseDto> SearchMinimalAsync(
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
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = 1;
        if (limit > 100) limit = 100;

        var query = context.KinguinProducts
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .AsQueryable();
        query = ApplyFilters(query, name, platform, regionId, isPreorder, tags, genre, updatedSince);
        query = ApplyOrdering(query, sortBy, sortType);

        var itemCount = await query.CountAsync(cancellationToken);
        var entities = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);

        return new KinguinProductListResponseDto
        {
            ItemCount = itemCount,
            Results = entities.Select(e => e.MapToListItemDto()).ToList()
        };
    }

    public async Task<KinguinProductListResponseDto> SearchMinimalAsync(
        Expression<Func<KinguinProduct, bool>>? filter,
        int page = 1,
        int limit = 25,
        Func<IQueryable<KinguinProduct>, IOrderedQueryable<KinguinProduct>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = 1;
        if (limit > 100) limit = 100;

        IQueryable<KinguinProduct> query = context.KinguinProducts.AsNoTracking().Where(p => !p.IsDeleted);
        if (filter != null) query = query.Where(filter);
        if (orderBy != null) query = orderBy(query); else query = query.OrderByDescending(p => p.UpdatedAt);

        var itemCount = await query.CountAsync(cancellationToken);
        var entities = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new KinguinProductListResponseDto
        {
            ItemCount = itemCount,
            Results = entities.Select(e => e.MapToListItemDto()).ToList()
        };
    }

    public async Task<KinguinProductDto?> GetByProductId(string productId, CancellationToken cancellationToken = default)
    {
        var entity = await context.KinguinProducts.AsNoTracking()
            .Where(p => !p.IsDeleted && p.ProductId == productId)
            .FirstOrDefaultAsync(cancellationToken);
        return entity?.MapToDto();
    }

    public async Task<Result> DeleteAsync(string productId, CancellationToken cancellationToken = default)
    {
        var product = await context.KinguinProducts.FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken: cancellationToken);
        if (product == null || product.IsDeleted)
        {
            return new Result()
            {
                Success = true,
                Message = "Product is Deleted"
            };
        }
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return new Result()
        {
            Success = true,
            Message = "Product Deleted"
        };
    }

    public async Task<KinguinProductListResponseDto> GetDeletedProducts(int page = 1, int limit = 25,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = 1;
        if (limit > 100) limit = 100;
        var query = context.KinguinProducts
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted);
        var itemCount = await query.CountAsync(cancellationToken);
        var entities = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new KinguinProductListResponseDto
        {
            ItemCount = itemCount,
            Results = entities.Select(e => e.MapToListItemDto()).ToList()
        };
    }

    public async Task<Result> RestoreAsync(string productId, CancellationToken cancellationToken = default)
    {
        var product = await context.KinguinProducts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken: cancellationToken);
        if (product == null)
        {
            return new Result()
            {
                Success = false,
                Message = "Product Not Found"
            };
        }
        product.IsDeleted = false;
        product.DeletedAt = null;
        await context.SaveChangesAsync(cancellationToken);
        return new Result()
        {
            Success = true,
            Message = "Product Restored"
        };
    }
}


