using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.webhooks;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/admin/webhooks")]
[Authorize(Roles = "Admin")]
public class WebhookAnalyticsController : ControllerBase
{
    private readonly IWebhookAnalyticsService _analyticsService;
    private readonly ILogger<WebhookAnalyticsController> _logger;

    public WebhookAnalyticsController(
        IWebhookAnalyticsService analyticsService,
        ILogger<WebhookAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive webhook analytics dashboard data
    /// </summary>
    [HttpGet("analytics")]
    public async Task<ActionResult<WebhookAnalyticsDto>> GetAnalytics()
    {
        try
        {
            var analytics = await _analyticsService.GetAnalyticsAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving webhook analytics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get paginated list of webhook events with filtering
    /// </summary>
    [HttpGet("events")]
    public async Task<ActionResult<PaginatedResponse<WebhookEventSummaryDto>>> GetWebhookEvents(
        [FromQuery] WebhookEventFilterDto filter)
    {
        try
        {
            var events = await _analyticsService.GetWebhookEventsAsync(filter);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving webhook events");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get specific webhook event details by ID
    /// </summary>
    [HttpGet("events/{id}")]
    public async Task<ActionResult<WebhookEventSummaryDto>> GetWebhookEvent(int id)
    {
        try
        {
            var webhookEvent = await _analyticsService.GetWebhookEventByIdAsync(id);
            if (webhookEvent == null)
            {
                return NotFound($"Webhook event with ID {id} not found");
            }
            return Ok(webhookEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving webhook event {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get hourly webhook statistics for a date range
    /// </summary>
    [HttpGet("stats/hourly")]
    public async Task<ActionResult<List<WebhookStatsByHourDto>>> GetHourlyStats(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        try
        {
            if (fromDate >= toDate)
            {
                return BadRequest("fromDate must be before toDate");
            }

            var stats = await _analyticsService.GetHourlyStatsAsync(fromDate, toDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hourly webhook stats");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get daily webhook statistics for a date range
    /// </summary>
    [HttpGet("stats/daily")]
    public async Task<ActionResult<List<WebhookStatsByDayDto>>> GetDailyStats(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        try
        {
            if (fromDate >= toDate)
            {
                return BadRequest("fromDate must be before toDate");
            }

            var stats = await _analyticsService.GetDailyStatsAsync(fromDate, toDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving daily webhook stats");
            return StatusCode(500, "Internal server error");
        }
    }
}
