using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class WholesaleInfoDto
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("tiers")]
    public List<WholesaleTierDto> Tiers { get; set; } = new();
}