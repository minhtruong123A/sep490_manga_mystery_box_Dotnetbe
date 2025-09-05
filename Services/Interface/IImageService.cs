using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Services.Interface;

public interface IImageService
{
    Task<IActionResult> GetImageWithWatermarkAsync(string path);
    Task WarmUpImageCacheAsync();
    Task<string> UploadProfileImageAsync(IFormFile file);
    Task DeleteProfileImageAsync(string oldFileName);
    Task<string> UploadModeratorProductOrMysteryBoxImageAsync(IFormFile file);
}