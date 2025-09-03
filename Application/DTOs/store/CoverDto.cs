using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class CoverDto
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; } = string.Empty;
}