using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class SystemRequirementDto
{
    [JsonPropertyName("system")] public string System { get; set; } = string.Empty;

    [JsonPropertyName("requirement")] public List<string> Requirement { get; set; } = new();
}
