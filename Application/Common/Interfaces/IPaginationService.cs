using System.Linq.Expressions;

namespace Application.Common.Interfaces;

public interface IPaginationService<TEntity> where TEntity : class
{
    Task<PaginatedResult<TEntity>> GetPaginatedAsync<TResult>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> selector,
        int page = 1,
        int limit = 25,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<TEntity>> GetPaginatedAsync(
        IQueryable<TEntity> query,
        int page = 1,
        int limit = 25,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default);
}

public class PaginatedResult<TEntity> where TEntity : class
{
    public List<TEntity> Results { get; set; } = new();
    public int ItemCount { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)ItemCount / Limit);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
