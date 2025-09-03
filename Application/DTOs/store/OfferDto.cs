using System.Text.Json.Serialization;

namespace Application.DTOs.store;

public class OfferDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("offerId")]
    public string OfferId { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("qty")]
    public int Qty { get; set; }

    [JsonPropertyName("availableQty")]
    public int? AvailableQty { get; set; }

    [JsonPropertyName("availableTextQty")]
    public int? AvailableTextQty { get; set; }

    [JsonPropertyName("textQty")]
    public int? TextQty { get; set; }

    [JsonPropertyName("merchantName")]
    public string? MerchantName { get; set; }

    [JsonPropertyName("isPreorder")]
    public bool IsPreorder { get; set; }

    [JsonPropertyName("releaseDate")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("wholesale")]
    public WholesaleInfoDto? Wholesale { get; set; }
}