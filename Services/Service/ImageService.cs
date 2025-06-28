using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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

namespace Services.Service
{
    public class ImageService : IImageService
    {
        private readonly ISupabaseStorageHelper _supabaseStorageHelper;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private record CachedImage(byte[] Content, string ContentType);

        public ImageService(ISupabaseStorageHelper supabaseStorageHelper, IMemoryCache cache, IUnitOfWork unitOfWork)
        {
            _supabaseStorageHelper = supabaseStorageHelper;
            _httpClient = new HttpClient();
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

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
            string watermarkText = $"MMB{id}";
            FontFamily fontFamily = SystemFonts.Families.FirstOrDefault(f => f.Name == "Arial");
            if (fontFamily == default(FontFamily)) fontFamily = SystemFonts.Families.First();

            int dynamicFontSize = image.Width / 70;
            dynamicFontSize = Math.Clamp(dynamicFontSize, 10, 40);
            Font font = fontFamily.CreateFont(dynamicFontSize, FontStyle.Regular);

            var brush = Brushes.Solid(SixLaborsColor.White.WithAlpha(0.4f));
            var textOptions = new RichTextOptions(font){ HorizontalAlignment = HorizontalAlignment.Center, };
            FontRectangle size = TextMeasurer.MeasureBounds(watermarkText, textOptions);
            float centerX = (image.Width - size.Width) / 2;
            float centerY = (image.Height - size.Height) / 2;
            float offsetX = image.Width * 0.12f;
            float offsetY = image.Height * 0.12f;
            textOptions.Origin = new PointF(centerX + offsetX, centerY + offsetY);
            image.Mutate(ctx => ctx.DrawText(textOptions, watermarkText, brush));
        }
    }
}