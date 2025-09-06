using System.Linq.Expressions;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PaginationService<TEntity> : IPaginationService<TEntity> where TEntity : class
{
    public async Task<PaginatedResult<TEntity>> GetPaginatedAsync<TResult>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> selector,
        int page = 1,
        int limit = 25,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        // Apply filter if provided
        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Apply ordering if provided, otherwise use default ordering
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var itemCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var skip = (page - 1) * limit;
        var results = await query.Skip(skip).Take(limit).ToListAsync(cancellationToken);

        return new PaginatedResult<TEntity>
        {
            Results = results,
            ItemCount = itemCount,
            Page = page,
            Limit = limit
        };
    }

    public async Task<PaginatedResult<TEntity>> GetPaginatedAsync(
        IQueryable<TEntity> query,
        int page = 1,
        int limit = 25,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        return await GetPaginatedAsync(query, x => x, page, limit, filter, orderBy, cancellationToken);
    }
}
