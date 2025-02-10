using System.Data;
using Infrastructure.DataAccess.Repositories;

namespace Infrastructure.DataAccess;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public IRefreshTokenRepository RefreshTokenRepository { get; }
    public Task<int> CommitAsync();
    public Task<IDbTransaction> BeginTransactionAsync();
}