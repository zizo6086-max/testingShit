using System.Text.Json.Serialization;

namespace Application.DTOs.store;

    public class KinguinProductDto
    {
        [JsonPropertyName("kinguinId")]
        public int KinguinId { get; set; }

        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("originalName")]
        public string? OriginalName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("developers")]
        public List<string> Developers { get; set; } = new();

        [JsonPropertyName("publishers")]
        public List<string> Publishers { get; set; } = new();

        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; } = new();

        [JsonPropertyName("platform")]
        public string? Platform { get; set; }

        [JsonPropertyName("releaseDate")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("qty")]
        public int Qty { get; set; }

        [JsonPropertyName("textQty")]
        public int TextQty { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("cheapestOfferId")]
        public List<string> CheapestOfferId { get; set; } = new();

        [JsonPropertyName("isPreorder")]
        public bool IsPreorder { get; set; }

        [JsonPropertyName("metacriticScore")]
        public float? MetacriticScore { get; set; }

        [JsonPropertyName("regionalLimitations")]
        public string? RegionalLimitations { get; set; }

        [JsonPropertyName("countryLimitation")]
        public List<string> CountryLimitation { get; set; } = new();

        [JsonPropertyName("regionId")]
        public int? RegionId { get; set; }

        [JsonPropertyName("activationDetails")]
        public string? ActivationDetails { get; set; }

        [JsonPropertyName("videos")]
        public List<VideoInfoDto> Videos { get; set; } = new();

        [JsonPropertyName("languages")]
        public List<string> Languages { get; set; } = new();

        [JsonPropertyName("systemRequirements")]
        public List<SystemRequirementDto> SystemRequirements { get; set; } = new();

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("offers")]
        public List<OfferDto> Offers { get; set; } = new();

        [JsonPropertyName("offersCount")]
        public int OffersCount { get; set; }

        [JsonPropertyName("totalQty")]
        public int TotalQty { get; set; }

        [JsonPropertyName("merchantName")]
        public List<string> MerchantName { get; set; } = new();

        [JsonPropertyName("ageRating")]
        public string? AgeRating { get; set; }

        [JsonPropertyName("steam")]
        public string? Steam { get; set; }

        [JsonPropertyName("images")]
        public ProductImagesDto? Images { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }