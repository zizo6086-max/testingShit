namespace Application.DTOs;

/// <summary>
/// Configuration options for photo upload functionality
/// </summary>
public class PhotoOptions
{
    // Defines allowed file extensions for photo uploads
    public List<string> AllowedFileFormats { get; set; } =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    ];
    
    // Maximum allowed file size (default: 5MB)
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024;
    
    // Maximum number of simultaneous uploads allowed
    public int MaxConcurrentUploads { get; set; } = 10;
    
    // Base directory path for storing uploaded files
    public string BaseUploadPath { get; set; } = "uploads";
}