using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class ProductImagesDto
{
    [JsonPropertyName("screenshots")]
    public List<ScreenshotDto> Screenshots { get; set; } = new();

    [JsonPropertyName("cover")]
    public CoverDto? Cover { get; set; }
}