using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.webhooks;
using Domain.Models.Store;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IWebhookAnalyticsService
{
    Task<WebhookAnalyticsDto> GetAnalyticsAsync();
    Task<PaginatedResponse<WebhookEventSummaryDto>> GetWebhookEventsAsync(WebhookEventFilterDto filter);
    Task<WebhookEventSummaryDto?> GetWebhookEventByIdAsync(int id);
    Task<List<WebhookStatsByHourDto>> GetHourlyStatsAsync(DateTime fromDate, DateTime toDate);
    Task<List<WebhookStatsByDayDto>> GetDailyStatsAsync(DateTime fromDate, DateTime toDate);
}

public class WebhookAnalyticsService(AppDbContext context, ILogger<WebhookAnalyticsService> logger)
    : IWebhookAnalyticsService
{
    private readonly ILogger<WebhookAnalyticsService> _logger = logger;

    public async Task<WebhookAnalyticsDto> GetAnalyticsAsync()
    {
        var totalWebhooks = await context.WebhookEvents.CountAsync();
        var successfulWebhooks = await context.WebhookEvents.CountAsync(w => w.Status == WebhookEventStatus.Success);
        var failedWebhooks = await context.WebhookEvents.CountAsync(w => w.Status == WebhookEventStatus.Failed);
        var pendingWebhooks = await context.WebhookEvents.CountAsync(w => w.Status == WebhookEventStatus.Pending || w.Status == WebhookEventStatus.Processing);

        var successRate = totalWebhooks > 0 ? (double)successfulWebhooks / totalWebhooks * 100 : 0;

        var averageProcessingTime = await context.WebhookEvents
            .Where(w => w.Status == WebhookEventStatus.Success && w.ProcessingTimeMs > 0)
            .AverageAsync(w => (double?)w.ProcessingTimeMs) ?? 0;

        var lastWebhookReceived = await context.WebhookEvents
            .OrderByDescending(w => w.ReceivedAt)
            .Select(w => w.ReceivedAt)
            .FirstOrDefaultAsync();

        var recentEvents = await context.WebhookEvents
            .OrderByDescending(w => w.ReceivedAt)
            .Take(10)
            .Select(w => new WebhookEventSummaryDto
            {
                Id = w.Id,
                EventType = w.EventType,
                Source = w.Source,
                ProductId = w.ProductId,
                KinguinId = w.KinguinId,
                Status = w.Status,
                ErrorMessage = w.ErrorMessage,
                ProcessingTimeMs = w.ProcessingTimeMs,
                ReceivedAt = w.ReceivedAt,
                ProcessedAt = w.ProcessedAt
            })
            .ToListAsync();

        // Get hourly stats for last 24 hours
        var fromDate = DateTime.UtcNow.AddHours(-24);
        var hourlyStats = await GetHourlyStatsAsync(fromDate, DateTime.UtcNow);

        // Get daily stats for last 7 days
        var dailyFromDate = DateTime.UtcNow.AddDays(-7);
        var dailyStats = await GetDailyStatsAsync(dailyFromDate, DateTime.UtcNow);

        return new WebhookAnalyticsDto
        {
            TotalWebhooks = totalWebhooks,
            SuccessfulWebhooks = successfulWebhooks,
            FailedWebhooks = failedWebhooks,
            PendingWebhooks = pendingWebhooks,
            SuccessRate = Math.Round(successRate, 2),
            AverageProcessingTimeMs = Math.Round(averageProcessingTime, 2),
            LastWebhookReceived = lastWebhookReceived,
            RecentEvents = recentEvents,
            HourlyStats = hourlyStats,
            DailyStats = dailyStats
        };
    }

    public async Task<PaginatedResponse<WebhookEventSummaryDto>> GetWebhookEventsAsync(WebhookEventFilterDto filter)
    {
        var query = context.WebhookEvents.AsQueryable();

        // Apply filters
        if (filter.Status.HasValue)
            query = query.Where(w => w.Status == filter.Status.Value);

        if (!string.IsNullOrEmpty(filter.EventType))
            query = query.Where(w => w.EventType == filter.EventType);

        if (!string.IsNullOrEmpty(filter.Source))
            query = query.Where(w => w.Source == filter.Source);

        if (filter.FromDate.HasValue)
            query = query.Where(w => w.ReceivedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(w => w.ReceivedAt <= filter.ToDate.Value);

        var totalCount = await query.CountAsync();

        var events = await query
            .OrderByDescending(w => w.ReceivedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(w => new WebhookEventSummaryDto
            {
                Id = w.Id,
                EventType = w.EventType,
                Source = w.Source,
                ProductId = w.ProductId,
                KinguinId = w.KinguinId,
                Status = w.Status,
                ErrorMessage = w.ErrorMessage,
                ProcessingTimeMs = w.ProcessingTimeMs,
                ReceivedAt = w.ReceivedAt,
                ProcessedAt = w.ProcessedAt
            })
            .ToListAsync();

        return new PaginatedResponse<WebhookEventSummaryDto>
        {
            Results = events,
            ItemCount = totalCount
        };
    }

    public async Task<WebhookEventSummaryDto?> GetWebhookEventByIdAsync(int id)
    {
        var webhookEvent = await context.WebhookEvents
            .Where(w => w.Id == id)
            .Select(w => new WebhookEventSummaryDto
            {
                Id = w.Id,
                EventType = w.EventType,
                Source = w.Source,
                ProductId = w.ProductId,
                KinguinId = w.KinguinId,
                Status = w.Status,
                ErrorMessage = w.ErrorMessage,
                ProcessingTimeMs = w.ProcessingTimeMs,
                ReceivedAt = w.ReceivedAt,
                ProcessedAt = w.ProcessedAt
            })
            .FirstOrDefaultAsync();

        return webhookEvent;
    }

    public async Task<List<WebhookStatsByHourDto>> GetHourlyStatsAsync(DateTime fromDate, DateTime toDate)
    {
        var stats = await context.WebhookEvents
            .Where(w => w.ReceivedAt >= fromDate && w.ReceivedAt <= toDate)
            .GroupBy(w => new { 
                Hour = new DateTime(w.ReceivedAt.Year, w.ReceivedAt.Month, w.ReceivedAt.Day, w.ReceivedAt.Hour, 0, 0)
            })
            .Select(g => new WebhookStatsByHourDto
            {
                Hour = g.Key.Hour,
                TotalCount = g.Count(),
                SuccessCount = g.Count(w => w.Status == WebhookEventStatus.Success),
                FailedCount = g.Count(w => w.Status == WebhookEventStatus.Failed),
                AverageProcessingTimeMs = g.Where(w => w.Status == WebhookEventStatus.Success && w.ProcessingTimeMs > 0)
                    .Average(w => (double?)w.ProcessingTimeMs) ?? 0
            })
            .OrderBy(s => s.Hour)
            .ToListAsync();

        return stats;
    }

    public async Task<List<WebhookStatsByDayDto>> GetDailyStatsAsync(DateTime fromDate, DateTime toDate)
    {
        var stats = await context.WebhookEvents
            .Where(w => w.ReceivedAt >= fromDate && w.ReceivedAt <= toDate)
            .GroupBy(w => w.ReceivedAt.Date)
            .Select(g => new WebhookStatsByDayDto
            {
                Date = g.Key,
                TotalCount = g.Count(),
                SuccessCount = g.Count(w => w.Status == WebhookEventStatus.Success),
                FailedCount = g.Count(w => w.Status == WebhookEventStatus.Failed),
                AverageProcessingTimeMs = g.Where(w => w.Status == WebhookEventStatus.Success && w.ProcessingTimeMs > 0)
                    .Average(w => (double?)w.ProcessingTimeMs) ?? 0
            })
            .OrderBy(s => s.Date)
            .ToListAsync();

        return stats;
    }
}
