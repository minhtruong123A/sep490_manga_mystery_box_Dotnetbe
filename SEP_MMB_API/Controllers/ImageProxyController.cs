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

        public ImageProxyController(IImageService imageService)
        {
            _imageService = imageService;
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
    }
}