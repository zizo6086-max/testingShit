using System.Security.Claims;
using Application.DTOs;
using Application.Services;
using Domain.Models;
using Domain.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleAuthController(
    AuthService authService,
    UserManager<AppUser> userManager,
    ILogger<GoogleAuthController> logger)
    : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback)),
            Items =
            {
                { "returnUrl", returnUrl }
            }
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            logger.LogError("External authentication error");
            return BadRequest(new { Message = "External authentication error" });
        }
        
        var externalUser = authenticateResult.Principal;
        var googleId = externalUser.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = externalUser.FindFirstValue(ClaimTypes.Email);
        var name = externalUser.FindFirstValue(ClaimTypes.Name);
        var authResult = await authService.HandleGoogleAuthCallbackAsync(googleId, email, name);
        if (!authResult.Success)
        {
            return BadRequest(authResult);
        }

        return Ok(authResult);
    }
}