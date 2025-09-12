using System.Text.Json.Serialization;

namespace Application.DTOs.webhooks;

public class KinguinProductUpdateWebhookDto
{
    [JsonPropertyName("kinguinId")]
    public int KinguinId { get; set; }

    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("qty")]
    public int Qty { get; set; }

    [JsonPropertyName("textQty")]
    public int TextQty { get; set; }

    [JsonPropertyName("cheapestOfferId")]
    public List<string> CheapestOfferId { get; set; } = new();

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
