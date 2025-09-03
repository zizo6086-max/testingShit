namespace Infrastructure.Configuration;

public class KinguinSettings
{
    public const string SectionName = "Kinguin";
    
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://gateway.kinguin.net/esa/api/v1";
}
