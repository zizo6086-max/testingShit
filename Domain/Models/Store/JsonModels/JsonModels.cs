namespace Domain.Models.Store.JsonModels;

public class VideoInfo
{
    public string VideoId { get; set; } = string.Empty;
}

public class SystemRequirement
{
    public string System { get; set; } = string.Empty;
    public List<string> Requirement { get; set; } = new();
}

public class ProductImages
{
    public List<Screenshot> Screenshots { get; set; } = new();
    public Cover? Cover { get; set; }
}

public class Screenshot
{
    public string Url { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
}

public class Cover
{
    public string Url { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
}

public class ProductOffer
{
    public string Name { get; set; } = string.Empty;
    public string OfferId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Qty { get; set; }
    public int? AvailableQty { get; set; }
    public int? AvailableTextQty { get; set; }
    public int? TextQty { get; set; }
    public string? MerchantName { get; set; }
    public bool IsPreorder { get; set; }
    public string? ReleaseDate { get; set; }
    public WholesaleInfo? Wholesale { get; set; }
}

public class WholesaleInfo
{
    public bool Enabled { get; set; }
    public List<WholesaleTier> Tiers { get; set; } = new();
}

public class WholesaleTier
{
    public int Level { get; set; }
    public decimal Price { get; set; }
}