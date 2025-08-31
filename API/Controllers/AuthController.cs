using System.Security.Claims;
using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService,ILogger<AuthController> logger, IEmailService emailService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await authService.RegisterAsync(registerDto, AuthConstants.Roles.User);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await authService.LoginAsync(loginDto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> RevokeToken(string refreshToken)
    {
        try
        {
            var success = await authService.RevokeTokenAsync(refreshToken, "Logged out");
            if (success)
            {
                return Ok();
            }
            return BadRequest();
        }
        catch (SecurityTokenException e)
        {
            return Unauthorized(new {e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "An unexpected error occured during Revoke token");
            return StatusCode(500, new {Message = "error occured during Revoke token"});
        }
    }
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(string refreshToken)
    {
        try
        {
            var result = await authService.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }
        catch (SecurityTokenException e)
        {
            return Unauthorized(new { e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "An unexpected error occured during Refresh token");
            return StatusCode(500, new {Message = "error occured during Refresh token"});
        }
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
    {
        var result = await emailService.VerifyEmailAsync(userId, token);
        if (result.Success)
        {
            return Redirect("https://uzer-zone.vercel.app/");
        }
        return BadRequest(result);
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] EmailVerificationRequest model)
    {
        var result = await emailService.SendEmailVerificationAsync(model); 
        if(result.Success)
            return Ok(result);
        if(result.Message == "User not found")
            return BadRequest(result);
        return StatusCode(500,result);
    }

}