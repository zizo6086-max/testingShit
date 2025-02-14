using System.Security.Claims;
using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController(UserManager<AppUser> userManager): ControllerBase
{
    [HttpPost("ChangePassword")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var user = await userManager.FindByIdAsync(
            User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        if (user == null)
            return BadRequest(new {Message = "Error Getting User To change the Password"});
        var result = await userManager.ChangePasswordAsync(user,dto.OldPassword,dto.NewPassword);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);
    }
    
}