using System.ComponentModel.DataAnnotations;
using Application.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services;

/// <summary>
/// Implementation of IPhotoService that handles photo upload and deletion operations
/// </summary>
public class PhotoService
{
    private readonly IWebHostEnvironment _webHostEnvironment;    // Provides information about the web hosting environment
    private readonly ILogger<PhotoService> _logger;             // Logger for recording service operations
    private readonly PhotoOptions _options;               // Configuration options for the service
    private readonly SemaphoreSlim _uploadSemaphore;           // Controls concurrent upload operations

    /// <summary>
    /// Constructor initializing required dependencies and configuration
    /// </summary>
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
    /// Saves multiple photos to the specified directory
    /// </summary>
    /// <param name="files">Collection of files to upload</param>
    /// <param name="directoryName">Target directory for uploads</param>
    /// <returns>Tuple containing list of file URLs and any errors encountered</returns>
    public async Task<(List<string> FileUrls, List<string> Errors)> SavePhotosAsync(
        IEnumerable<IFormFile> files,
        string directoryName)
    {
        var fileUrls = new List<string>();
        var errors = new List<string>();

        try
        {
            // Validate input parameters
            var formFiles = files.ToList();
            await ValidateInputsAsync(formFiles, directoryName);

            // Get or create upload directory
            var uploadDirectory = GetUploadDirectory(directoryName);
            
            // Process each file upload concurrently
            var uploadTasks = formFiles.Select(file => ProcessFileUploadAsync(file, uploadDirectory, fileUrls, errors));
            await Task.WhenAll(uploadTasks);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for file upload");
            errors.Add(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in SavePhotosAsync");
            errors.Add("An unexpected server error occurred. Please try again later.");
        }

        return (fileUrls, errors);
    }
    
    public async Task<bool> DeletePhotoAsync(string fileUrl)
    {
        try
        {
            // Convert URL to physical file path
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, fileUrl.TrimStart('/'));
            
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Attempted to delete non-existent file: {FileUrl}", fileUrl);
                return false;
            }
            
            await Task.Run(() => File.Delete(filePath));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            return false;
        }
    }


    private async Task ValidateInputsAsync(IEnumerable<IFormFile> files, string directoryName)
    {
        var formFiles = files.ToList();
        if (files == null || formFiles.Count == 0)
            throw new ValidationException("No files provided.");

        if (string.IsNullOrWhiteSpace(directoryName))
            throw new ValidationException("Directory name cannot be empty.");

        if (formFiles.Count > _options.MaxConcurrentUploads)
            throw new ValidationException($"Maximum number of files exceeded. Limit is {_options.MaxConcurrentUploads}.");

        await ValidateDirectoryPathAsync(directoryName);
    }
    
    private string GetUploadDirectory(string directoryName)
    {
        var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, _options.BaseUploadPath, directoryName);
        
        if (!Directory.Exists(uploadDirectory))
        {
            Directory.CreateDirectory(uploadDirectory);
        }

        return uploadDirectory;
    }
    
    private async Task ProcessFileUploadAsync(
        IFormFile file,
        string uploadDirectory,
        List<string> fileUrls,
        List<string> errors)
    {
        try
        {
            await _uploadSemaphore.WaitAsync();
            if (!ValidateFile(file, out var validationError))
            {
                errors.Add(validationError);
                return;
            }

            var (fileName, filePath) = GenerateUniqueFilePath(file, uploadDirectory);
            await SaveFileAsync(file, filePath);
            var fileUrl = $"/{_options.BaseUploadPath}/{fileName}";
            lock (fileUrls)
            {
                fileUrls.Add(fileUrl);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file: {FileName}", file.FileName);
            lock (errors)
            {
                errors.Add($"Failed to process {file.FileName}: {ex.Message}");
            }
        }
        finally
        {
            _uploadSemaphore.Release();
        }
    }
    
    private bool ValidateFile(IFormFile file, out string error)
    {
        if (file.Length == 0)
        {
            error = $"File {file.FileName} is empty.";
            return false;
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!_options.AllowedFileFormats.Contains(fileExtension))
        {
            error = $"File {file.FileName} has an invalid format. Allowed formats: {string.Join(", ", _options.AllowedFileFormats)}";
            return false;
        }

        if (file.Length > _options.MaxFileSize)
        {
            error = $"File {file.FileName} exceeds the maximum allowed size of {_options.MaxFileSize / (1024 * 1024)} MB.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static (string fileName, string filePath) GenerateUniqueFilePath(
        IFormFile file,
        string uploadDirectory)
    {
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadDirectory, fileName);
        return (fileName, filePath);
    }

    private static async Task SaveFileAsync(IFormFile file, string filePath)
    {
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }
    
    private async Task ValidateDirectoryPathAsync(string directoryName)
    {
        // Prevent directory traversal attacks
        if (directoryName.Contains(".."))
            throw new ValidationException("Invalid directory path.");

        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, _options.BaseUploadPath, directoryName);
        var fullPathInfo = new DirectoryInfo(fullPath);
        var webRootInfo = new DirectoryInfo(_webHostEnvironment.WebRootPath);

        // Ensure the target directory is under webroot for security
        if (!fullPathInfo.FullName.StartsWith(webRootInfo.FullName))
            throw new ValidationException("Invalid directory path.");

        await Task.CompletedTask;
    }
}