using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class VideoInfoDto
{
    [JsonPropertyName("video_id")]
    public string VideoId { get; set; } = string.Empty;
}