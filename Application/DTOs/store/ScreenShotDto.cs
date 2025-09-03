using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class ScreenshotDto
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; } = string.Empty;
}