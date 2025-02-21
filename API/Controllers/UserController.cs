using System.Security.Claims;
using Application.DTOs;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController(UserService userService, UserManager<AppUser> userManager,PhotoService photoService,ILogger<UserController> logger): ControllerBase
{
    [HttpGet("Me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await userService.GetUserInfoAsync(userId);
        return Ok(result);
    }
    [HttpPut("ChangePassword")]
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

    [HttpPut("UpdateProfilePicture")]
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

    [HttpDelete("DeleteProfilePicture")]
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
}