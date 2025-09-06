using Application.Common.Interfaces;
using Application.DTOs.store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Constants;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController(IApplicationsService applicationsService) : ControllerBase
{
    [HttpPost()]
    [Authorize(Roles = AuthConstants.Roles.User)]
    public async Task<IActionResult> SubmitApplication([FromBody] SellerApplicationDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await applicationsService.SubmitApplicationAsync(userId, dto);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
    
    [HttpGet()]
    [Authorize(Roles = AuthConstants.Roles.Admin)]
    public async Task<IActionResult> SearchSellerApplications(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 25,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var response = await applicationsService.SearchSellerApplicationsAsync(
            page, 
            limit, 
            status, 
            cancellationToken);
        
        return Ok(response);
    }
}