using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("RegisterUser")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await authService.RegisterAsync(registerDto,"User");
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("LoginUser")]
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

    [HttpPost("RevokeToken")]
    public async Task<IActionResult> RevokeToken(string refreshToken)
    {
        var result = await authService.RevokeTokenAsync(refreshToken,"Ra5ama");
        if (result)
        {
            return BadRequest();
        }

        return Ok();
    }
}