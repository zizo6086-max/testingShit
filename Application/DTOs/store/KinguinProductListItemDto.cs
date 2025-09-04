using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class KinguinProductListItemDto
{
    [JsonPropertyName("kinguinId")]
    public int KinguinId { get; set; }

    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("originalName")]
    public string? OriginalName { get; set; }

    [JsonPropertyName("platform")]
    public string? Platform { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("offersCount")]
    public int OffersCount { get; set; }

    [JsonPropertyName("totalQty")]
    public int TotalQty { get; set; }

    [JsonPropertyName("isPreorder")]
    public bool IsPreorder { get; set; }

    [JsonPropertyName("metacriticScore")]
    public float? MetacriticScore { get; set; }

    [JsonPropertyName("images")]
    public ProductImagesDto? Images { get; set; }
}


