using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class KinguinProductSearchResponseDto
{
    [JsonPropertyName("results")]
    public List<KinguinProductDto> Results { get; set; } = new();

    [JsonPropertyName("item_count")]
    public int ItemCount { get; set; }
}