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
        var result = await roleService.RemoveSellerRole(userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}