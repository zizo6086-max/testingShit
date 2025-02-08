using System.Data;
using Infrastructure.DataAccess.Repositories;

namespace Infrastructure.DataAccess;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public Task<int> CommitAsync();
    public Task<IDbTransaction> BeginTransactionAsync();
}