using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class WholesaleTierDto
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}