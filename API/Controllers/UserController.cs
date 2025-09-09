using System.Security.Claims;
using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Constants;
using Domain.Models;
using Domain.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, UserManager<AppUser> userManager, IPhotoService photoService,ILogger<UserController> logger): ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await userService.GetUserInfoAsync(userId);
        return Ok(result);
    }
    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var result = await userService.ChangePasswordAsync(userId, dto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut("update-profile-picture")]
    [Authorize]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile file)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value!;
        var result = await userService.UpdateProfilePictureAsync(userId, file);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("delete-profile-picture")]
    [Authorize]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value!;
        var result = await userService.DeleteProfilePictureAsync(userId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet]
    [Authorize(Roles = AuthConstants.Roles.Admin)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] string? role,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 25,
        [FromQuery] string sortBy = "Role",
        [FromQuery] string sortType = "asc")
    {
        var result = 
            await userService.GetAllUsersAsync(role, page, limit, sortBy, sortType);
        return Ok(result);
    }
}