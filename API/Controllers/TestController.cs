using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TestController:ControllerBase
{
    [HttpGet("OnlyAuthUsers")]
    [Authorize]
    public IActionResult OnlyAuthUsers()
    {
        return Ok("You are logged in");
    }

    [HttpGet("OnlyAdminUsers")]
    [Authorize(Roles = "Admin")]
    public IActionResult OnlyAdminUsers()
    {
        return Ok("You are logged in as Admin");
    }
}