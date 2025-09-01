namespace Infrastructure.Configuration;

public class KinguinSettings
{
    public const string SectionName = "Kinguin";
    public string SandBoxKey { get; set; } = string.Empty;
    public string ProductionKey { get; set; } = string.Empty;
}