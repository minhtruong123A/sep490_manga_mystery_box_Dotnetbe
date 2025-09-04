using System.Text.RegularExpressions;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Interface;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLaborsColor = SixLabors.ImageSharp.Color;
using SixLaborsImage = SixLabors.ImageSharp.Image;

namespace Services.Service;

public class ImageService(
    ISupabaseStorageHelper supabaseStorageHelper,
    IMemoryCache cache,
    IUnitOfWork unitOfWork,
    ILogger<ImageService> logger,
    IOptions<ImageProxySettings> proxySettings)
    : IImageService
{
    private static readonly FontCollection _fontCollection = new();
    private static readonly FontFamily _customFontFamily;
    private readonly HttpClient _httpClient = new();
    private readonly ImageProxySettings _proxySettings = proxySettings.Value;

    static ImageService()
    {
        var fontPath = Path.Combine(AppContext.BaseDirectory, "Fonts", "arial.ttf");
        if (File.Exists(fontPath)) _customFontFamily = _fontCollection.Add(fontPath);
    }

    //cache warm-up
    public async Task WarmUpImageCacheAsync()
    {
        var mangaBoxes = await unitOfWork.MangaBoxRepository.GetAllWithDetailsAsync();
        var sellProducts = await unitOfWork.SellProductRepository.GetAllProductOnSaleAsync();
        var imageUrls = mangaBoxes.Select(m => m.UrlImage)
            .Concat(sellProducts.Select(p => p.UrlImage))
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Distinct()
            .ToList();
        logger.LogInformation($"[WarmUp] Found {imageUrls.Count} image URLs to preload.");
        if (cache is MemoryCache memCache)
        {
            memCache.Compact(0.2);
            logger.LogInformation("[WarmUp] Performed memory cache compaction.");
        }

        var throttler = new SemaphoreSlim(5);
        var tasks = new List<Task>();

        foreach (var url in imageUrls)
        {
            var finalKey = $"final_image_{url}";
            if (cache.TryGetValue(finalKey, out _))
            {
                logger.LogInformation($"[WarmUp] Skipped cached image: {url}");
                continue;
            }

            await throttler.WaitAsync();
            var task = Task.Run(async () =>
            {
                try
                {
                    var encodedPath = Uri.EscapeDataString(url);
                    //var proxyUrl = $"https://mmb-be-dotnet.onrender.com/api/ImageProxy/{encodedPath}";
                    //var proxyUrls = new[]
                    //{
                    //    $"https://mmb-be-dotnet.onrender.com/api/ImageProxy/{encodedPath}",
                    //    $"https://api.mmb.io.vn/cs/api/ImageProxy/{encodedPath}"
                    //};  
                    var proxyUrls = _proxySettings.BaseUrls
                        .Select(baseUrl => $"{baseUrl.TrimEnd('/')}/{_proxySettings.Path.Trim('/')}/{encodedPath}")
                        .ToArray();

                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(50));
                    //await _httpClient.GetAsync(proxyUrl, cts.Token);
                    var warmUpTasks = proxyUrls.Select(async proxyUrl =>
                    {
                        try
                        {
                            logger.LogInformation($"[WarmUp] Sending request to: {proxyUrl}");
                            var response = await _httpClient.GetAsync(proxyUrl, cts.Token);
                            response.EnsureSuccessStatusCode();
                            logger.LogInformation($"[WarmUp] Success: {proxyUrl}");
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning($"[WarmUp] Failed to warm {proxyUrl}: {ex.Message}");
                        }
                    });

                    await Task.WhenAll(warmUpTasks);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"[WarmUp] Image warm-up failed for {url}: {ex.Message}");
                }
                finally
                {
                    throttler.Release();
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        logger.LogInformation("[WarmUp] Image cache warm-up completed.");
    }

    // get watermark async
    public async Task<IActionResult> GetImageWithWatermarkAsync(string path)
    {
        var finalImageCacheKey = $"final_image_{path}";
        if (cache.TryGetValue(finalImageCacheKey, out CachedImage cachedImage))
            return new FileContentResult(cachedImage.Content, cachedImage.ContentType);

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
            using (var image = await SixLaborsImage.LoadAsync(imageStream))
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
        cache.Set(finalImageCacheKey, new CachedImage(finalImageBytes, contentType), cacheEntryOptions);
        return new FileContentResult(finalImageBytes, contentType);
    }

    // upload profile image
    public async Task<string> UploadProfileImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) throw new Exception("No file uploaded");

        var filePath = await supabaseStorageHelper.UploadImageAsync(file);

        return filePath;
    }

    //upload moderator product or mysterybox image
    public async Task<string> UploadModeratorProductOrMysteryBoxImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) throw new Exception("No file uploaded");

        var fileName = file.FileName;
        var finalFileName = await GenerateNextFileNameAsync(fileName);
        var uploadedPath = await supabaseStorageHelper.UploadSystemProductImageAsync(file, finalFileName);

        return uploadedPath;
    }

    //private static bool IsValidSystemProductFileName(string fileName)
    //{
    //    var cardPattern = @"^(Common|Uncommon|Rare|Epic|Legendary)CardNo\d+[A-Za-z0-9]+\.png$";
    //    var boxsetPattern = @"^[A-Za-z0-9]+_Boxset\.png$";

    //    return Regex.IsMatch(fileName, cardPattern, RegexOptions.IgnoreCase) || Regex.IsMatch(fileName, boxsetPattern, RegexOptions.IgnoreCase);
    //}

    // delete profile image
    public async Task DeleteProfileImageAsync(string oldFileName)
    {
        if (!string.IsNullOrWhiteSpace(oldFileName)) await supabaseStorageHelper.DeleteImageAsync(oldFileName);
    }

    private async Task<HttpResponseMessage> GetImageResponseAsync(string path)
    {
        var cacheKey = $"signed_url_{path}";
        if (!cache.TryGetValue(cacheKey, out string signedUrl))
        {
            signedUrl = await supabaseStorageHelper.CreateSignedUrlAsync(path);
            var cacheOptions = new MemoryCacheEntryOptions
                { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4) };
            cache.Set(cacheKey, signedUrl, cacheOptions);
        }

        return await _httpClient.GetAsync(signedUrl);
    }

    private async Task<string> GetObjectIdFromDb(string path)
    {
        var cacheKey = $"db_object_id_{path}";
        if (cache.TryGetValue(cacheKey, out string cachedId)) return cachedId;

        var product = await unitOfWork.ProductRepository.FindOneAsync(p => p.UrlImage == path);
        if (product != null)
        {
            cache.Set(cacheKey, product.Id, TimeSpan.FromMinutes(10));
            return product.Id;
        }

        var mysteryBox = await unitOfWork.MangaBoxRepository.FindMysteryBoxByUrlImageAsync(path);
        if (mysteryBox != null)
        {
            cache.Set(cacheKey, mysteryBox.Id, TimeSpan.FromMinutes(10));
            return mysteryBox.Id;
        }

        cache.Set(cacheKey, string.Empty, TimeSpan.FromMinutes(10));
        return null;
    }

    private void ApplyIdWatermark(SixLaborsImage image, string id)
    {
        if (_customFontFamily == null) return;

        var watermarkText = $"MMB{id}";
        var dynamicFontSize = Math.Clamp(image.Width / 70, 10, 40);
        var font = _customFontFamily.CreateFont(dynamicFontSize, FontStyle.Regular);
        var brush = Brushes.Solid(SixLaborsColor.White.WithAlpha(0.4f));
        var textOptions = new RichTextOptions(font) { HorizontalAlignment = HorizontalAlignment.Center };
        var size = TextMeasurer.MeasureBounds(watermarkText, textOptions);
        var centerX = (image.Width - size.Width) / 2;
        var centerY = (image.Height - size.Height) / 2;
        var offsetX = image.Width * 0.12f;
        var offsetY = image.Height * 0.12f;
        textOptions.Origin = new PointF(centerX + offsetX, centerY + offsetY);
        image.Mutate(ctx => ctx.DrawText(textOptions, watermarkText, brush));
    }

    private async Task<string> GenerateNextFileNameAsync(string originalFileName)
    {
        var baseName = Path.GetFileNameWithoutExtension(originalFileName);
        var extension = Path.GetExtension(originalFileName);
        var cardPatternWithNumber =
            new Regex(@"^(?<rarity>Common|Uncommon|Rare|Epic|Legendary)CardNo(?<number>\d+)(?<manga>[A-Za-z0-9]+)$",
                RegexOptions.IgnoreCase);
        var cardPatternWithoutNumber =
            new Regex(@"^(?<rarity>Common|Uncommon|Rare|Epic|Legendary)Card(?<manga>[A-Za-z0-9]+)$",
                RegexOptions.IgnoreCase);
        var boxsetPattern = new Regex(@"^[A-Za-z0-9]+_Boxset$", RegexOptions.IgnoreCase);
        if (cardPatternWithNumber.IsMatch(baseName))
        {
            if (await supabaseStorageHelper.FileExistsAsync(originalFileName))
                throw new Exception($"A file with the name '{originalFileName}' already exists.");

            return originalFileName;
        }

        var match = cardPatternWithoutNumber.Match(baseName);
        if (match.Success)
        {
            var rarity = match.Groups["rarity"].Value;
            var manga = match.Groups["manga"].Value;
            var allFiles = await supabaseStorageHelper.ListFilesByPrefixAsync(""); // Lấy tất cả
            var matchingFiles = allFiles
                .Where(f =>
                {
                    var name = Path.GetFileNameWithoutExtension(f);

                    return name.StartsWith($"{rarity}CardNo", StringComparison.OrdinalIgnoreCase) &&
                           name.EndsWith(manga, StringComparison.OrdinalIgnoreCase);
                }).ToList();

            var maxNumber = 0;
            var numberExtractor = new Regex($@"^{rarity}CardNo(?<number>\d+){manga}$", RegexOptions.IgnoreCase);

            foreach (var file in matchingFiles)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var matchNum = numberExtractor.Match(name);
                if (matchNum.Success && int.TryParse(matchNum.Groups["number"].Value, out var num))
                    if (num > maxNumber)
                        maxNumber = num;
            }

            var nextNumber = maxNumber + 1;
            return $"{rarity}CardNo{nextNumber}{manga}{extension}";
        }

        if (boxsetPattern.IsMatch(baseName))
        {
            if (await supabaseStorageHelper.FileExistsAsync(originalFileName))
                throw new Exception($"A file with the name '{originalFileName}' already exists.");

            return originalFileName;
        }

        throw new Exception(
            "Invalid file name format.\n" +
            "Accepted formats:\n" +
            "- '[Rarity]CardNo[Number][MangaName].png' (e.g., 'RareCardNo2OnePiece.png')\n" +
            "- '[Rarity]Card[MangaName].png' (e.g., 'RareCardOnePiece.png')\n" +
            "- '[MangaName]_Boxset.png' (e.g., 'TokyoGhoul_Boxset.png')\n\n" +
            "Rarity must be one of: Common, Uncommon, Rare, Epic, Legendary.\n" +
            "Number must be digits only.\n" +
            "MangaName must be alphanumeric (no spaces or special characters)."
        );
    }

    private record CachedImage(byte[] Content, string ContentType);
}