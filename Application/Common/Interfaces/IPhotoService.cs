using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface IPhotoService
{
    Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string directoryName);
    Task<bool> DeletePhotoAsync(string fileUrl);
}
