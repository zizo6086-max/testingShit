using Application.DTOs;

namespace Application.Common.Interfaces;

public interface IRoleService
{
    public Task<Result> AddSellerAsync(int userId);
    Task<Result> RemoveSellerAsync(int userId);
    Task<Result> AddAdminAsync(int userId, int adminId);
    Task<Result> RemoveAdminAsync(int userId, int adminId);
}