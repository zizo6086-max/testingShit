using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Constants;
using Domain.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class RoleService(
    ILogger<RoleService> logger,
    RoleManager<IdentityRole<int>> roleManager,
    UserManager<AppUser> userManager)
    : IRoleService
{
    public async Task<Result> AddSellerAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new Result()
            {
                Message = "User not found"
            };
        }
        var result = await userManager.AddToRoleAsync(user, AuthConstants.Roles.Seller);
        if (!result.Succeeded)
            return new Result()
            {
                Message = "Failed to add Seller Role to User"
            };
        logger.LogInformation("User added to seller role");
        return new Result()
        {
            Success = true,
            Message = "User added to seller role"
        };

    }

    public async Task<Result> RemoveSellerRole(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new Result()
            {
                Message = "User not found"
            };
        }
        var userRoles = await userManager.GetRolesAsync(user);
        if (!userRoles.Contains(AuthConstants.Roles.Seller))
        {
            return new Result()
            {
                Message = "User does not have seller role"
            };
        }
        var result = await userManager.RemoveFromRoleAsync(user, AuthConstants.Roles.Seller);
        if (!result.Succeeded)
        {
            return new Result()
            {
                Message = "Failed to remove seller role from user"
            };
        }
        return new Result()
        {
            Success = true,
            Message = "User removed seller role"
        };
    }
}