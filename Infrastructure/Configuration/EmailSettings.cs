namespace Infrastructure.Configuration;

public class EmailSettings
{
    public const string SectionName = "Email";
    
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}
