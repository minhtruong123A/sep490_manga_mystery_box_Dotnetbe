using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Interface;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLaborsColor = SixLabors.ImageSharp.Color;
using SixLaborsImage = SixLabors.ImageSharp.Image;
using DataAccessLayers.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Services.Service
{
    public class ImageService : IImageService
    {
        private readonly ISupabaseStorageHelper _supabaseStorageHelper;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ImageService> _logger;
        private record CachedImage(byte[] Content, string ContentType);
        private static readonly FontCollection _fontCollection = new();
        private static readonly FontFamily _customFontFamily;
        static ImageService()
        {
            var fontPath = Path.Combine(AppContext.BaseDirectory, "Fonts", "arial.ttf");
            if (File.Exists(fontPath))
            {
                _customFontFamily = _fontCollection.Add(fontPath);
            }
        }

        public ImageService(ISupabaseStorageHelper supabaseStorageHelper, IMemoryCache cache, IUnitOfWork unitOfWork, ILogger<ImageService> logger)
        {
            _supabaseStorageHelper = supabaseStorageHelper;
            _httpClient = new HttpClient();
            _cache = cache;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        //cache warm-up
        public async Task WarmUpImageCacheAsync()
        {
            var mangaBoxes = await _unitOfWork.MangaBoxRepository.GetAllWithDetailsAsync();
            var sellProducts = await _unitOfWork.SellProductRepository.GetAllProductOnSaleAsync();
            var imageUrls = mangaBoxes.Select(m => m.UrlImage)
                .Concat(sellProducts.Select(p => p.UrlImage))
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Distinct()
                .ToList();
            _logger.LogInformation($"[WarmUp] Found {imageUrls.Count} image URLs to preload.");
            if (_cache is MemoryCache memCache)
            {
                memCache.Compact(0.2);
                _logger.LogInformation("[WarmUp] Performed memory cache compaction.");
            }

            var throttler = new SemaphoreSlim(5);
            var tasks = new List<Task>();

            foreach (var url in imageUrls)
            {
                var finalKey = $"final_image_{url}";
                if (_cache.TryGetValue(finalKey, out _))
                {
                    _logger.LogInformation($"[WarmUp] Skipped cached image: {url}");
                    continue;
                }

                await throttler.WaitAsync();
                var task = Task.Run(async () =>
                {
                    try
                    {
                        var encodedPath = Uri.EscapeDataString(url);
                        //var proxyUrl = $"https://mmb-be-dotnet.onrender.com/api/ImageProxy/{encodedPath}";
                        var proxyUrls = new[]
                        {
                            $"https://mmb-be-dotnet.onrender.com/api/ImageProxy/{encodedPath}",
                            $"https://api.mmb.io.vn/cs/api/ImageProxy/{encodedPath}"
                        };  

                        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(50));
                        //await _httpClient.GetAsync(proxyUrl, cts.Token);
                        var warmUpTasks = proxyUrls.Select(async proxyUrl =>
                        {
                            try
                            {
                                _logger.LogInformation($"[WarmUp] Sending request to: {proxyUrl}");
                                var response = await _httpClient.GetAsync(proxyUrl, cts.Token);
                                response.EnsureSuccessStatusCode();
                                _logger.LogInformation($"[WarmUp] Success: {proxyUrl}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"[WarmUp] Failed to warm {proxyUrl}: {ex.Message}");
                            }
                        });

                        await Task.WhenAll(warmUpTasks);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"[WarmUp] Image warm-up failed for {url}: {ex.Message}");
                    }
                    finally
                    {
                        throttler.Release();
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            _logger.LogInformation($"[WarmUp] Image cache warm-up completed.");
        }

        // get watermark async
        public async Task<IActionResult> GetImageWithWatermarkAsync(string path)
        {
            var finalImageCacheKey = $"final_image_{path}";
            if (_cache.TryGetValue(finalImageCacheKey, out CachedImage cachedImage)) return new FileContentResult(cachedImage.Content, cachedImage.ContentType);

            var imageResponseTask = GetImageResponseAsync(path);
            var objectIdTask = GetObjectIdFromDb(path);
            await Task.WhenAll(imageResponseTask, objectIdTask);
            var response = await imageResponseTask;
            var objectId = await objectIdTask;
            if (!response.IsSuccessStatusCode) return new StatusCodeResult((int)response.StatusCode);

            var imageStream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";
            byte[] finalImageBytes;
            if (!string.IsNullOrEmpty(objectId))
            {
                using (SixLaborsImage image = await SixLaborsImage.LoadAsync(imageStream))
                {
                    ApplyIdWatermark(image, objectId);
                    using var ms = new MemoryStream();
                    await image.SaveAsync(ms, new PngEncoder());
                    finalImageBytes = ms.ToArray();
                }
            }
            else
            {
                using var ms = new MemoryStream();
                await imageStream.CopyToAsync(ms);
                finalImageBytes = ms.ToArray();
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(4));
            _cache.Set(finalImageCacheKey, new CachedImage(finalImageBytes, contentType), cacheEntryOptions);
            return new FileContentResult(finalImageBytes, contentType);
        }

        private async Task<HttpResponseMessage> GetImageResponseAsync(string path)
        {
            var cacheKey = $"signed_url_{path}";
            if (!_cache.TryGetValue(cacheKey, out string signedUrl))
            {
                signedUrl = await _supabaseStorageHelper.CreateSignedUrlAsync(path);
                var cacheOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4) };
                _cache.Set(cacheKey, signedUrl, cacheOptions);
            }

            return await _httpClient.GetAsync(signedUrl);
        }

        private async Task<string> GetObjectIdFromDb(string path)
        {
            var cacheKey = $"db_object_id_{path}";
            if (_cache.TryGetValue(cacheKey, out string cachedId)) return cachedId;

            var product = await _unitOfWork.ProductRepository.FindOneAsync(p => p.UrlImage == path);
            if (product != null)
            {
                _cache.Set(cacheKey, product.Id, TimeSpan.FromMinutes(10));
                return product.Id;
            }

            var mysteryBox = await _unitOfWork.MangaBoxRepository.FindMysteryBoxByUrlImageAsync(path);
            if (mysteryBox != null)
            {
                _cache.Set(cacheKey, mysteryBox.Id, TimeSpan.FromMinutes(10));
                return mysteryBox.Id;
            }

            _cache.Set(cacheKey, string.Empty, TimeSpan.FromMinutes(10));
            return null;
        }

        private void ApplyIdWatermark(SixLaborsImage image, string id)
        {
            if (_customFontFamily == null) return;

            string watermarkText = $"MMB{id}";
            int dynamicFontSize = Math.Clamp(image.Width / 70, 10, 40);
            Font font = _customFontFamily.CreateFont(dynamicFontSize, FontStyle.Regular);
            var brush = Brushes.Solid(SixLaborsColor.White.WithAlpha(0.4f));
            var textOptions = new RichTextOptions(font) { HorizontalAlignment = HorizontalAlignment.Center };
            FontRectangle size = TextMeasurer.MeasureBounds(watermarkText, textOptions);
            float centerX = (image.Width - size.Width) / 2;
            float centerY = (image.Height - size.Height) / 2;
            float offsetX = image.Width * 0.12f;
            float offsetY = image.Height * 0.12f;
            textOptions.Origin = new PointF(centerX + offsetX, centerY + offsetY);
            image.Mutate(ctx => ctx.DrawText(textOptions, watermarkText, brush));
        }

        public async Task<string> UploadProfileImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new Exception("No file uploaded");

            var filePath = await _supabaseStorageHelper.UploadImageAsync(file);

            return filePath;
        }
    }
}