using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Models;
using Domain.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserService(UserManager<AppUser> userManager,
    ILogger<UserService> logger,
    IPhotoService photoService) : IUserService
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
}