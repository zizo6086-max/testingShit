using Application.Common.Interfaces;
using Application.DTOs.store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class StoreConfigController(ILogger<StoreConfigController> logger, IStoreConfigService storeConfigService)
    : ControllerBase
{
    private readonly ILogger<StoreConfigController> _logger = logger;
    private readonly IStoreConfigService _storeConfigService = storeConfigService;

    [HttpPut("price-margin")]
    public async Task<IActionResult> SetPriceMargin(SetPriceMarginDto dto)
    {
        var result = await _storeConfigService.SetPriceMarginAsync(dto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}