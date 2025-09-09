using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserInfoAsync(int userId);
    Task<Result> UpdateProfilePictureAsync(string userId, IFormFile file);
    Task<Result> DeleteProfilePictureAsync(string userId);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    Task<UserListResponseDto> GetAllUsersAsync(string? role,
         int page = 1,
         int limit = 25,
         string sortBy = "Id",
         string sortType = "asc");

}
