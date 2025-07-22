using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;
using System;
using System.Threading.Tasks;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageProxyController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IAuthService _authService;
        private readonly ILogger<ImageProxyController> _logger;

        public ImageProxyController(IImageService imageService, ILogger<ImageProxyController> logger, IAuthService authService)
        {
            _imageService = imageService;
            _logger = logger;
            _authService = authService;
        }

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
                return await _imageService.GetImageWithWatermarkAsync(path);
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
                var filePath = await _imageService.UploadModeratorProductOrMysteryBoxImageAsync(file);

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
                _ = Task.Run(() => _imageService.WarmUpImageCacheAsync());

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