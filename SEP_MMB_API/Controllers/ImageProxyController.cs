using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageProxyController : ControllerBase
    {
        private readonly ISupabaseStorageHelper _supabaseStorageHelper;
        private readonly HttpClient _httpClient;

        public ImageProxyController(ISupabaseStorageHelper supabaseStorageHelper)
        {
            _supabaseStorageHelper = supabaseStorageHelper;
            _httpClient = new HttpClient();
        }

        [HttpGet("{*path}")]
        public async Task<IActionResult> ProxyImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return BadRequest("Path is required.");

            try
            {
                var signedUrl = await _supabaseStorageHelper.CreateSignedUrlAsync(path);

                Console.WriteLine($"[Signed URL] = {signedUrl}");

                var response = await _httpClient.GetAsync(signedUrl);
                if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, "Failed to fetch image from Supabase.");

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";
                var stream = await response.Content.ReadAsStreamAsync();

                return File(stream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Proxy failed: {ex.Message}");
            }
        }
    }
}
