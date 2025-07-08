using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System;
using System.Threading.Tasks;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageProxyController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageProxyController> _logger;

        public ImageProxyController(IImageService imageService, ILogger<ImageProxyController> logger)
        {
            _imageService = imageService;
            _logger = logger;
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

        [HttpGet("warmup-image-cache")]
        public async Task<IActionResult> WarmupImageCache()
        {
            try
            {
                await _imageService.WarmUpImageCacheAsync();

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