using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IKinguinProductQueryService queryService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 25,
        [FromQuery] string? name = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortType = null,
        [FromQuery] string? platform = null,
        [FromQuery] int? regionId = null,
        [FromQuery] string? isPreorder = null,
        [FromQuery] string? tags = null,
        [FromQuery] string? genre = null,
        [FromQuery] string? updatedSince = null,
        CancellationToken cancellationToken = default)
    {
        var response = await queryService.SearchMinimalAsync(page, limit, name, platform, regionId, isPreorder, tags, genre, updatedSince, sortBy, sortType, cancellationToken);
        return Ok(response);
    }
    
    [HttpGet("{productId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByProductId([FromRoute] string productId, CancellationToken cancellationToken)
    {
        var dto = await queryService.GetByProductId(productId, cancellationToken);
        if (dto == null) return NotFound();
        return Ok(dto);
    }
}


