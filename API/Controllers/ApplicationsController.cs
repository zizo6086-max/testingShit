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

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetByApplicationId()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await applicationsService.GetApplicationAsync(userId);
        return Ok(result);
    }

    [HttpPost("Deny-Application/{applicationId:int}")]
    [Authorize(Roles = AuthConstants.Roles.Admin)]
    public async Task<IActionResult> DenyApplication([FromRoute] int applicationId)
    {
        var result = await applicationsService.DenyApplicationAsync(applicationId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
    [HttpPost("Approve-Application/{applicationId:int}")]
    [Authorize (Roles = AuthConstants.Roles.Admin)]
    public async Task<IActionResult> ApproveApplication([FromRoute] int applicationId)
    {
        var result = await applicationsService.ApproveApplicationAsync(applicationId);
        if (result.Success)
        {
            return Ok(result);
        } 
        return BadRequest(result);
    }
    [HttpPost("Delete-Application/{applicationId:int}")]
    [Authorize(Roles = AuthConstants.Roles.Admin)]
    public async Task<IActionResult> DeleteApplication([FromRoute] int applicationId)
    {
        var result = await applicationsService.DeleteApplicationAsync(applicationId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}