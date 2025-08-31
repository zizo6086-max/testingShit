using Application.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services;

/// <summary>
/// handles photo upload and deletion operations
/// </summary>
public class PhotoService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<PhotoService> _logger;            
    private readonly PhotoOptions _options;               
    private readonly SemaphoreSlim _uploadSemaphore;           
    public PhotoService(
        IWebHostEnvironment webHostEnvironment,
        ILogger<PhotoService> logger,
        IOptions<PhotoOptions> options)
    {
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
        _options = options.Value;
        _uploadSemaphore = new SemaphoreSlim(_options.MaxConcurrentUploads);
    }
    /// <summary>
    /// Uploads a list of files to the specified directory and returns URLs for database storage
    /// </summary>
    /// <param name="files">List of files to upload</param>
    /// <param name="directoryName">Name of the directory to store files in</param>
    /// <returns>List of URLs for the uploaded files</returns>
    public async Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string directoryName)
    {
        var formFiles = files.ToList();
        if (formFiles.Count == 0)
        {
            _logger.LogWarning("No files provided for upload to directory: {DirectoryName}", directoryName);
            return [];
        }

        try
        {
            await _uploadSemaphore.WaitAsync();
            
            var urls = new List<string>();
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", directoryName);
            
            // Ensure directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
                _logger.LogInformation("Created directory: {DirectoryPath}", uploadPath);
            }

            foreach (var file in formFiles)
            {
                if (file.Length > 0)
                {
                    if (!IsValidFileType(file))
                    {
                        _logger.LogWarning("Skipped invalid file type: {FileName}", file.FileName);
                        continue;
                    }
                    // Generate a unique filename to prevent overwriting
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);
                    
                    // Save the file
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    
                    // Generate URL for the file (relative to web root)
                    var fileUrl = $"/uploads/{directoryName}/{fileName}";
                    urls.Add(fileUrl);
                    
                    _logger.LogInformation("File uploaded successfully: {FileName} to {FilePath}", fileName, filePath);
                }
                else
                {
                    _logger.LogWarning("Skipped empty file: {FileName}", file.FileName);
                }
            }
            
            return urls;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading files to directory: {DirectoryName}", directoryName);
            throw;
        }
        finally
        {
            _uploadSemaphore.Release();
        }
    }
    public async Task<bool> DeletePhotoAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            _logger.LogWarning("No file URL provided for deletion");
            return false;
        }
    
        try
        {
            // Extract the relative path from the URL
            var relativePath = fileUrl.TrimStart('/');
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
        
            if (File.Exists(filePath))
            {
                // Use asynchronous file operations
                await Task.Run(() => File.Delete(filePath));
                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
            
                // Check if directory is empty and delete if needed
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                    return true;
                var hasFiles = await Task.Run(() => Directory.EnumerateFileSystemEntries(directoryPath).Any());
                if (hasFiles)
                    return true;
                await Task.Run(() => Directory.Delete(directoryPath));
                _logger.LogInformation("Empty directory deleted: {DirectoryPath}", directoryPath);

                return true;
            }
            _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            return false;
        }
    }
    private bool IsValidFileType(IFormFile file)
    {
        if (_options.AllowedFileFormats.Count == 0)
        {
            _logger.LogWarning("No allowed file types configured. Allowing all files.");
            return true;
        }

        // Check by content type (MIME type)
        if (!string.IsNullOrEmpty(file.ContentType) && 
            _options.AllowedFileFormats.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        // Check by file extension as fallback
        var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (!string.IsNullOrEmpty(fileExtension))
        {
            // Remove the dot from extension for comparison
            var extensionWithoutDot = fileExtension.TrimStart('.');
            
            // Check if any allowed type ends with this extension (for cases like "image/jpeg" -> "jpeg")
            if (_options.AllowedFileFormats.Any(allowedType => 
                    allowedType.ToLowerInvariant().EndsWith(extensionWithoutDot) ||
                    allowedType.ToLowerInvariant() == fileExtension))
            {
                return true;
            }
        }

        return false;
    }
}