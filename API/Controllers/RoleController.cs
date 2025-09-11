using System.Security.Claims;
using Application.Common.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/roles")]
[ApiController]
[Authorize(Roles = AuthConstants.Roles.Admin)]
public class RoleController(IRoleService roleService) : ControllerBase
{
    [HttpPost("Add-Seller/{userId:int}")]
    public async Task<IActionResult> AddSeller([FromRoute]int userId)
    {
        var result = await roleService.AddSellerAsync(userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpDelete("Remove-Seller/{userId:int}")]
    public async Task<IActionResult> RemoveSeller([FromRoute] int userId)
    {
        var result = await roleService.RemoveSellerAsync(userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("Add-Admin/{userId:int}")]
    [Authorize(Roles = AuthConstants.Roles.Admin)]
    public async Task<IActionResult> AddAdmin([FromRoute] int userId)
    {
        var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await roleService.AddAdminAsync(userId, adminId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("Remove-Admin/{userId:int}")]
    [Authorize(Roles = AuthConstants.Roles.Admin)]
    public async Task<IActionResult> RemoveAdmin([FromRoute] int userId)
    {
        var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await roleService.RemoveAdminAsync(userId, adminId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}