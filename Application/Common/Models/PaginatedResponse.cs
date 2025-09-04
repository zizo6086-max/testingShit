using System.Text.Json.Serialization;

namespace Application.Common.Models;

public class PaginatedResponse<TItem>
{
    [JsonPropertyName("results")]
    public List<TItem> Results { get; set; } = new();

    [JsonPropertyName("item_count")]
    public int ItemCount { get; set; }
}


