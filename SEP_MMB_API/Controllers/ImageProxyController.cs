using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageProxyController : ControllerBase
    {
        private readonly ISupabaseStorageHelper _supabaseStorageHelper;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        public ImageProxyController(ISupabaseStorageHelper supabaseStorageHelper, IMemoryCache cache)
        {
            _supabaseStorageHelper = supabaseStorageHelper;
            _httpClient = new HttpClient();
            _cache = cache;
        }

        [HttpGet("{*path}")]
        public async Task<ActionResult<ResponseModel<object>>> ProxyImage(string path)
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
                var cacheKey = $"signed_url_{path}";
                if (!_cache.TryGetValue(cacheKey, out string signedUrl))
                {
                    signedUrl = await _supabaseStorageHelper.CreateSignedUrlAsync(path);
                    var cacheOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4) };
                    _cache.Set(cacheKey, signedUrl, cacheOptions);
                }

                var response = await _httpClient.GetAsync(signedUrl);
                if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, "Failed to fetch image from Supabase.");

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";
                var stream = await response.Content.ReadAsStreamAsync();

                return File(stream, contentType);
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
