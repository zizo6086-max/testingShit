using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Constants;
using Domain.Models.Auth;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class RoleService(
    ILogger<RoleService> logger,
    RoleManager<IdentityRole<int>> roleManager,
    UserManager<AppUser> userManager, AppDbContext context)
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
        var userApplication = context.SellerApplications.FirstOrDefault(x => x.UserId == userId);
        if (userApplication != null)
            userApplication.Status = SellerApplicationConstants.Approved;
        await context.SaveChangesAsync();
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

    public async Task<Result> AddAdminAsync(int userId, int adminId)
    {
        if (adminId != 1)
        {
            return new Result()
            {
                Message = "Only Super admin can add admin"
            };
        }

        var user = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return new Result()
            {
                Message = "User not Found"
            };
        }
        var result = await userManager.AddToRoleAsync(user, AuthConstants.Roles.Admin);
        if (!result.Succeeded)
            return new Result()
            {
                Success = false,
                Message = "Failed to added to  Admin Role"
            };
        logger.LogInformation("User {UserUserName} added to admin role", user.UserName);
        return new Result()
        {
            Success = true,
            Message = "User added to admin role"
        };

    }

    public async Task<Result> RemoveAdminAsync(int userId, int adminId)
    {
        if (adminId != 1)
        {
            return new Result()
            {
                Message = "Only Super admin can Remove admin"
            };
        }

        var user = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return new Result()
            {
                Message = "User not Found"
            };
        }
        var userRoles = await userManager.GetRolesAsync(user);
        if (!userRoles.Contains(AuthConstants.Roles.Admin))
        {
            return new Result()
            {
                Success = true,
                Message = "User removed From Admin role"
            };
        }
        var result = await userManager.RemoveFromRoleAsync(user, AuthConstants.Roles.Admin);
        if (!result.Succeeded)
        {
            return new Result()
            {
                Message = "Failed to remove admin role"
            };
        }
        logger.LogInformation("User {UserUserName} removed From admin role", user.UserName);
        return new Result()
        {
            Success = true,
            Message = "User removed From Admin role"
        };

    }
}