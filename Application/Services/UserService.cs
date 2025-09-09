using Application.Common.Interfaces;
using Application.DTOs;
using Application.Mappers;
using Domain.Models;
using Domain.Models.Auth;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserService(UserManager<AppUser> userManager,
    ILogger<UserService> logger,
    IPhotoService photoService, AppDbContext context,
    IPaginationService<AppUser> paginationService) : IUserService
{
    public async Task<UserDto> GetUserInfoAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new UserDto();
        }

        var result = new UserDto()
        {
            UserName = user.UserName,
            Email = user.Email,
            ImageUrl = user.ImageUrl,
            IsVerified = user.EmailConfirmed
        };
        return result;
    }
    public async Task<Result> UpdateProfilePictureAsync(string userId, IFormFile file)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User does not exist"
                };
            }

            if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                await photoService.DeletePhotoAsync(user.ImageUrl);
            }

            var fileUrls = await photoService.UploadFilesAsync(new[] { file }, "Users");
            if (fileUrls.Count == 0)
            {
                return new Result()
                {
                    Message = "Failed to upload profile picture"
                };
            }
            user.ImageUrl = fileUrls[0];
            var updateResult = await userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
                return new Result()
                {
                    Success = true,
                    Message = "User Image updated",
                    Data = user.ImageUrl
                };
            await photoService.DeletePhotoAsync(user.ImageUrl);
            return new Result()
            {
                Message = "User could not be updated"
            };

        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error Updating user Image for {userId}");
            return new Result()
            {
                Message = "UnExpected Error Occurred Updating User Image "
            };
        }
    }

    public async Task<Result> DeleteProfilePictureAsync(string userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User does not exist"
                };
            }

            if (string.IsNullOrEmpty(user.ImageUrl))
            {
                return new Result()
                {
                    Success = true,
                    Message = "No Profile Picture available"
                };
            }

            var deleteResult = await photoService.DeletePhotoAsync(user.ImageUrl);
            if (!deleteResult)
            {
                return new Result()
                {
                    Message = "Failed to delete profile picture"
                };
            }

            user.ImageUrl = "";
            if (!(await userManager.UpdateAsync(user)).Succeeded)
            {
                return new Result()
                {
                    Message = "Failed to Update profile picture after Deleting it" // Improve later

                };
            }

            return new Result()
            {
                Success = true,
                Message = "Profile Picture deleted"
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error Deleting profile picture for {userId}");
            return new Result()
            {
                Message = "UnExpected Error Occurred Deleting profile picture"
            };
        }
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new Result()
            {
                Message = "User does not exist"
            };
        }
        var result = await userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            return new Result()
            {
                Message = "Failed to change password",
                Data = result.Errors
            };
        }
        return new Result()
        {
            Success = true,
            Message = "Changed Password"
        };
    }

 public async Task<UserListResponseDto> GetAllUsersAsync(
    string? role, 
    int page = 1, 
    int limit = 25, 
    string sortBy = "Id", 
    string sortType = "asc")
{
    // Base join
    var query =
        from u in context.Users
        join ur in context.UserRoles on u.Id equals ur.UserId
        join r in context.Roles on ur.RoleId equals r.Id
        select new { u, r.Name };

    // Filter by role if provided
    if (!string.IsNullOrEmpty(role))
    {
        query = query.Where(x => x.Name == role);
    }

    // Group by user, but defer materializing roles list to memory
    var grouped = query
        .GroupBy(x => x.u)
        .Select(g => new
        {
            User = g.Key,
            Roles = g.Select(x => x.Name) // keep as IEnumerable<string>
        });

    // Sorting (server-side)
    var sorted = (sortBy.ToLower(), sortType.ToLower()) switch
    {
        ("id", "asc")       => grouped.OrderBy(x => x.User.Id),
        ("id", "desc")      => grouped.OrderByDescending(x => x.User.Id),
        ("username", "asc") => grouped.OrderBy(x => x.User.UserName),
        ("username", "desc")=> grouped.OrderByDescending(x => x.User.UserName),
        ("email", "asc")    => grouped.OrderBy(x => x.User.Email),
        ("email", "desc")   => grouped.OrderByDescending(x => x.User.Email),
        _ => grouped.OrderBy(x => x.User.Id)
    };

    // Count before paging
    var totalCount = await sorted.CountAsync();

    // Paging + move to memory for roles list
    var result = sorted
        .Skip((page - 1) * limit)
        .Take(limit)
        .AsEnumerable() // switch to LINQ-to-Objects
        .Select(g => new UserListDto(
            g.User.Id,
            g.User.UserName,
            g.User.Email!,
            g.User.EmailConfirmed,
            g.User.CreatedOn.DateTime,
            g.Roles.ToList(), // now safe
            g.User.ImageUrl
        )).ToList();

    return new UserListResponseDto
    {
        Results = result,
        ItemCount = totalCount
    };
}

    public async Task<Result> GetUserAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new Result()
        {
            Message = "User not found"
        };
        var roles = await userManager.GetRolesAsync(user);
        var dto = new UserListDto(user.Id,
            user.UserName,
            user.Email!,
            user.EmailConfirmed,
            user.CreatedOn.DateTime,
            roles.ToList(),
            user.ImageUrl);
        return new Result()
        {
            Success = true,
            Message = "User Found",
            Data = dto
        };
    }
}