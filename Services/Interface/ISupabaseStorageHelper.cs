using Microsoft.AspNetCore.Http;

namespace Services.Interface;

public interface ISupabaseStorageHelper
{
    Task<string> CreateSignedUrlAsync(string path, int? expiresIn = null);
    Task<string> UploadImageAsync(IFormFile file);
    Task DeleteImageAsync(string fileName);
    Task<string> UploadSystemProductImageAsync(IFormFile file, string fileNameToUse);
    Task<bool> FileExistsAsync(string fileName);
    Task<List<string>> ListFilesByPrefixAsync(string prefix);
}