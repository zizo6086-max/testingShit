using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Store;

public class WebhookEvent
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string EventType { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Source { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ProductId { get; set; }

    public int? KinguinId { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? Payload { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? Headers { get; set; }

    [MaxLength(45)]
    public string? ClientIp { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public WebhookEventStatus Status { get; set; } = WebhookEventStatus.Pending;

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    public int ProcessingTimeMs { get; set; }

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum WebhookEventStatus
{
    Pending = 0,
    Processing = 1,
    Success = 2,
    Failed = 3,
    Retry = 4
}
