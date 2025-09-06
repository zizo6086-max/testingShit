using Application.DTOs;

namespace Application.Common.Interfaces;

public interface IRoleService
{
    public Task<Result> AddSellerAsync(int userId);
    Task<Result> RemoveSellerRole(int userId);
}