namespace Application.DTOs;

/// <summary>
/// Configuration options for photo upload functionality
/// </summary>
public class PhotoOptions
{
    // Defines allowed file extensions for photo uploads
    public IEnumerable<string> AllowedFileFormats { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    
    // Maximum allowed file size (default: 5MB)
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024;
    
    // Maximum number of simultaneous uploads allowed
    public int MaxConcurrentUploads { get; set; } = 10;
    
    // Base directory path for storing uploaded files
    public string BaseUploadPath { get; set; } = "uploads";
}