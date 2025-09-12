using Domain.Models.Store;

namespace Application.DTOs.webhooks;

public class WebhookAnalyticsDto
{
    public int TotalWebhooks { get; set; }
    public int SuccessfulWebhooks { get; set; }
    public int FailedWebhooks { get; set; }
    public int PendingWebhooks { get; set; }
    public double SuccessRate { get; set; }
    public double AverageProcessingTimeMs { get; set; }
    public DateTime LastWebhookReceived { get; set; }
    public List<WebhookEventSummaryDto> RecentEvents { get; set; } = new();
    public List<WebhookStatsByHourDto> HourlyStats { get; set; } = new();
    public List<WebhookStatsByDayDto> DailyStats { get; set; } = new();
}

public class WebhookEventSummaryDto
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public int? KinguinId { get; set; }
    public WebhookEventStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public int ProcessingTimeMs { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class WebhookStatsByHourDto
{
    public DateTime Hour { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public double AverageProcessingTimeMs { get; set; }
}

public class WebhookStatsByDayDto
{
    public DateTime Date { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public double AverageProcessingTimeMs { get; set; }
}

public class WebhookEventFilterDto
{
    public WebhookEventStatus? Status { get; set; }
    public string? EventType { get; set; }
    public string? Source { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
