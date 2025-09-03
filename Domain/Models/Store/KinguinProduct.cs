using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Store;
public class KinguinProduct
{
    [Key] public int Id { get; set; }

    // Primary identifiers from Kinguin API
    public int KinguinId { get; set; }

    [Required] [MaxLength(50)] public string ProductId { get; set; } = string.Empty;

    // Basic product information
    [Required] [MaxLength(500)] public string Name { get; set; } = string.Empty;

    [MaxLength(500)] public string? OriginalName { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? Description { get; set; }

    // Platform and genre information
    [MaxLength(100)] public string? Platform { get; set; }

    public DateTime? ReleaseDate { get; set; }

    // Pricing and availability
    [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }

    public int Qty { get; set; }
    public int TextQty { get; set; }
    public int TotalQty { get; set; }
    public int OffersCount { get; set; }

    // Pre-order information
    public bool IsPreorder { get; set; }

    // Regional information
    public int? RegionId { get; set; }

    [MaxLength(200)] public string? RegionalLimitations { get; set; }

    // Ratings and metadata
    public float? MetacriticScore { get; set; }

    [MaxLength(20)] public string? AgeRating { get; set; }

    [MaxLength(20)] public string? Steam { get; set; }

    // Large text fields
    [Column(TypeName = "nvarchar(max)")] public string? ActivationDetails { get; set; }

    // JSON columns for complex data
    [Column(TypeName = "nvarchar(max)")] public string? DevelopersJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? PublishersJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? GenresJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? LanguagesJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? TagsJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? CountryLimitationJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? MerchantNameJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? CheapestOfferIdJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? VideosJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? SystemRequirementsJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? ImagesJson { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string? OffersJson { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Track last API update
    public DateTime? LastApiUpdate { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}