using System.Text.Json;
using Application.DTOs.store;
using Domain.Models.Store;
using Domain.Models.Store.JsonModels;

namespace Application.Mappers;

    public static class KinguinProductMappingExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static KinguinProduct MapToEntity(this KinguinProductDto dto, KinguinProduct? existingProduct = null)
        {
            var product = existingProduct ?? new KinguinProduct();
            
            // Basic properties
            product.KinguinId = dto.KinguinId;
            product.ProductId = dto.ProductId;
            product.Name = dto.Name;
            product.OriginalName = dto.OriginalName;
            product.Description = dto.Description;
            product.Platform = dto.Platform;
            product.Price = dto.Price;
            product.Qty = dto.Qty;
            product.TextQty = dto.TextQty;
            product.TotalQty = dto.TotalQty;
            product.OffersCount = dto.OffersCount;
            product.IsPreorder = dto.IsPreorder;
            product.MetacriticScore = dto.MetacriticScore;
            product.RegionalLimitations = dto.RegionalLimitations;
            product.RegionId = dto.RegionId;
            product.ActivationDetails = dto.ActivationDetails;
            product.AgeRating = dto.AgeRating;
            product.Steam = dto.Steam;
            product.LastApiUpdate = DateTime.UtcNow;

            // Parse release date
            if (!string.IsNullOrEmpty(dto.ReleaseDate) && DateTime.TryParse(dto.ReleaseDate, out var releaseDate))
            {
                product.ReleaseDate = releaseDate;
            }

            // Parse updated date
            if (!string.IsNullOrEmpty(dto.UpdatedAt) && DateTime.TryParse(dto.UpdatedAt, out var updatedAt))
            {
                product.UpdatedAt = updatedAt;
            }

            // Serialize JSON fields
            product.DevelopersJson = JsonSerializer.Serialize(dto.Developers, JsonOptions);
            product.PublishersJson = JsonSerializer.Serialize(dto.Publishers, JsonOptions);
            product.GenresJson = JsonSerializer.Serialize(dto.Genres, JsonOptions);
            product.LanguagesJson = JsonSerializer.Serialize(dto.Languages, JsonOptions);
            product.TagsJson = JsonSerializer.Serialize(dto.Tags, JsonOptions);
            product.CountryLimitationJson = JsonSerializer.Serialize(dto.CountryLimitation, JsonOptions);
            product.MerchantNameJson = JsonSerializer.Serialize(dto.MerchantName, JsonOptions);
            product.CheapestOfferIdJson = JsonSerializer.Serialize(dto.CheapestOfferId, JsonOptions);

            // Convert and serialize complex objects
            var videos = dto.Videos.Select(v => new VideoInfo { VideoId = v.VideoId }).ToList();
            product.VideosJson = JsonSerializer.Serialize(videos, JsonOptions);

            var sysReqs = dto.SystemRequirements.Select(sr => new SystemRequirement 
            { 
                System = sr.System, 
                Requirement = sr.Requirement 
            }).ToList();
            product.SystemRequirementsJson = JsonSerializer.Serialize(sysReqs, JsonOptions);

            if (dto.Images != null)
            {
                var images = new ProductImages
                {
                    Screenshots = dto.Images.Screenshots.Select(s => new Screenshot 
                    { 
                        Url = s.Url, 
                        Thumbnail = s.Thumbnail 
                    }).ToList(),
                    Cover = dto.Images.Cover != null ? new Cover 
                    { 
                        Url = dto.Images.Cover.Url, 
                        Thumbnail = dto.Images.Cover.Thumbnail 
                    } : null
                };
                product.ImagesJson = JsonSerializer.Serialize(images, JsonOptions);
            }

            var offers = dto.Offers.Select(o => new ProductOffer
            {
                Name = o.Name,
                OfferId = o.OfferId,
                Price = o.Price,
                Qty = o.Qty,
                AvailableQty = o.AvailableQty,
                AvailableTextQty = o.AvailableTextQty,
                TextQty = o.TextQty,
                MerchantName = o.MerchantName,
                IsPreorder = o.IsPreorder,
                ReleaseDate = o.ReleaseDate,
                Wholesale = o.Wholesale != null ? new WholesaleInfo
                {
                    Enabled = o.Wholesale.Enabled,
                    Tiers = o.Wholesale.Tiers.Select(t => new WholesaleTier
                    {
                        Level = t.Level,
                        Price = t.Price
                    }).ToList()
                } : null
            }).ToList();
            product.OffersJson = JsonSerializer.Serialize(offers, JsonOptions);

            return product;
        }

        // Helper methods to deserialize JSON fields back to objects
        public static List<string> GetDevelopers(this KinguinProduct product)
        {
            return DeserializeJsonField<List<string>>(product.DevelopersJson) ?? new List<string>();
        }

        public static List<string> GetPublishers(this KinguinProduct product)
        {
            return DeserializeJsonField<List<string>>(product.PublishersJson) ?? new List<string>();
        }

        public static List<string> GetGenres(this KinguinProduct product)
        {
            return DeserializeJsonField<List<string>>(product.GenresJson) ?? new List<string>();
        }

        public static List<string> GetLanguages(this KinguinProduct product)
        {
            return DeserializeJsonField<List<string>>(product.LanguagesJson) ?? new List<string>();
        }

        public static List<string> GetTags(this KinguinProduct product)
        {
            return DeserializeJsonField<List<string>>(product.TagsJson) ?? new List<string>();
        }

        public static List<string> GetCountryLimitations(this KinguinProduct product)
        {
            return DeserializeJsonField<List<string>>(product.CountryLimitationJson) ?? new List<string>();
        }

        public static List<string> GetMerchantNames(this KinguinProduct product)
        {
            return DeserializeJsonField<List<string>>(product.MerchantNameJson) ?? new List<string>();
        }

        public static List<string> GetCheapestOfferIds(this KinguinProduct product)
        {
            return DeserializeJsonField<List<string>>(product.CheapestOfferIdJson) ?? new List<string>();
        }

        public static List<VideoInfo> GetVideos(this KinguinProduct product)
        {
            return DeserializeJsonField<List<VideoInfo>>(product.VideosJson) ?? new List<VideoInfo>();
        }

        public static List<SystemRequirement> GetSystemRequirements(this KinguinProduct product)
        {
            return DeserializeJsonField<List<SystemRequirement>>(product.SystemRequirementsJson) ?? new List<SystemRequirement>();
        }

        public static ProductImages? GetImages(this KinguinProduct product)
        {
            return DeserializeJsonField<ProductImages>(product.ImagesJson);
        }

        public static List<ProductOffer> GetOffers(this KinguinProduct product)
        {
            return DeserializeJsonField<List<ProductOffer>>(product.OffersJson) ?? new List<ProductOffer>();
        }

        private static T? DeserializeJsonField<T>(string? json) where T : class
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
            catch
            {
                return null;
            }
        }
    }