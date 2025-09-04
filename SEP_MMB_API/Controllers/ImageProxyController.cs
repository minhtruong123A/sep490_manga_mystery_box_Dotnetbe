using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageProxyController(
        IImageService imageService)
        : ControllerBase
    {
        [HttpGet("{*path}")]
        public async Task<IActionResult> ProxyImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = "Path is required.",
                    ErrorCode = 400
                });
            }

            try
            {
                return await imageService.GetImageWithWatermarkAsync(path);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = $"Proxy failed: {ex.Message}",
                    ErrorCode = 400
                });
            }
        }

        /*[Authorize]
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var filePath = await _imageService.UploadProfileImageAsync(file, account.Id);

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = filePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }*/

        [Authorize]
        [HttpPost("upload-card-or-boxset-image")]
        public async Task<IActionResult> UploadSystemProductImage(IFormFile file)
        {
            try
            {
                var filePath = await imageService.UploadModeratorProductOrMysteryBoxImageAsync(file);

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = filePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("warmup-image-cache")]
        public IActionResult WarmupImageCache()
        {
            try
            {
                _ = Task.Run(() => imageService.WarmUpImageCacheAsync());

                return Ok(new ResponseModel<object>
                {
                    Data = new { Message = "Image cache warm-up triggered successfully." },
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Data = null,
                    Success = false,
                    Error = $"Warm-up failed: {ex.Message}",
                    ErrorCode = 400
                });
            }
        }
    }
}