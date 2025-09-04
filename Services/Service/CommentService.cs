using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BusinessObjects;
using BusinessObjects.Dtos.Comment;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class CommentService(IUnitOfWork unitOfWork, IModerationService moderationService)
    : ICommentService
{
    private static readonly HashSet<string> BadWords = LoadBadWords();
    private static readonly HashSet<string> AllowedShortWords = LoadAllowedShortWords();

    public async Task<List<CommentWithUsernameDto>> GetAlCommentlBySellProductIdAsync(string sellProductId)
    {
        return await unitOfWork.CommentRepository.GetAlCommentlBySellProductIdAsync(sellProductId);
    }

    public async Task<List<RatingWithUsernameDto>> GetAllRatingBySellProductIdAsync(string sellProductId)
    {
        return await unitOfWork.CommentRepository.GetAllRatingBySellProductIdAsync(sellProductId);
    }

    //rating create and validation
    public async Task<Comment> CreateRatingOnlyAsync(string sellProductId, string userId, float rating)
    {
        if (string.IsNullOrWhiteSpace(sellProductId))
            throw new Exception("SellProductId must not be empty or whitespace.");

        if (rating is < 0 or > 5) throw new Exception("Rating must be between 0 and 5.");

        var product = await unitOfWork.SellProductRepository.GetByIdAsync(sellProductId);
        if (product == null || !product.IsSell) throw new Exception("Product not found or not available for sale.");

        var existingRatingOnly =
            await unitOfWork.CommentRepository.GetRatingOnlyByUserAndProductAsync(userId, sellProductId);
        if (existingRatingOnly == null)
            return await unitOfWork.CommentRepository.CreateRatingOnlyAsync(sellProductId, userId, rating);
        existingRatingOnly.Rating = rating;
        existingRatingOnly.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.CommentRepository.UpdateAsync(existingRatingOnly.Id, existingRatingOnly);
        return existingRatingOnly;
    }

    //comment create and validation
    public async Task<Comment> CreateCommentAsync(string sellProductId, string userId, string content)
    {
        var sanitizedContent = ValidateCommentInput(sellProductId, content);

        var product = await unitOfWork.SellProductRepository.GetByIdAsync(sellProductId);
        if (product is not { IsSell: true }) throw new Exception("Product not found or not available for sale.");

        var existingCommentOnly =
            await unitOfWork.CommentRepository.GetCommentOnlyByUserAndProductAsync(userId, sellProductId);
        if (existingCommentOnly == null)
            return await unitOfWork.CommentRepository.CreateCommentAsync(sellProductId, userId, sanitizedContent);
        existingCommentOnly.Content = sanitizedContent;
        existingCommentOnly.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.CommentRepository.UpdateAsync(existingCommentOnly.Id, existingCommentOnly);
        return existingCommentOnly;
    }

    // delete all comment
    public async Task DeleteAllCommentAsync()
    {
        await unitOfWork.CommentRepository.DeleteAllAsync();
    }

    // get all bad words
    public Task<List<string>> GetAllBadWordsAsync()
    {
        return Task.FromResult(BadWords.ToList());
    }

    public async Task<List<CommentWithUsernameDto>> GetAllCommentProductOfUserAsync(string userId, string productName)
    {
        return await unitOfWork.CommentRepository.GetAllCommentProductOfUserAsync(userId, productName);
    }

    public async Task<float> GetRatingOfUser(string userId)
    {
        return await unitOfWork.CommentRepository.GetRatingOfUserAsync(userId);
    }

    //public Task<List<string>> GetAllAllowedShortWordsAsync()
    //{
    //    return Task.FromResult(AllowedShortWords.ToList());
    //}
    public async Task<float> GetTotalAverageOfSellProductByIdAsync(string sellProductId)
    {
        return await unitOfWork.CommentRepository.GetTotalAverageOfSellProductByIdAsync(sellProductId);
    }

    //set up reading load file
    private static HashSet<string> LoadBadWords()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "badwords.txt");
        if (!File.Exists(path)) throw new FileNotFoundException($"File not found: {path}");

        return File.ReadAllLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(word => word.ToLower().Trim())
            .ToHashSet();
    }

    private static HashSet<string> LoadAllowedShortWords()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AllowedShortWords.txt");
        if (!File.Exists(path)) throw new FileNotFoundException($"File not found: {path}");

        return File.ReadAllLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(word => word.ToLower().Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private string ValidateCommentInput(string sellProductId, string content)
    {
        if (string.IsNullOrWhiteSpace(sellProductId)) throw new Exception("SellProductId must not be empty.");
        if (string.IsNullOrWhiteSpace(content)) throw new Exception("Comment content cannot be empty.");
        if (content.Length > 1000) throw new Exception("Comment content too long (max 1000 characters).");
        //bool isSafe = await _moderationService.IsContentSafeGeminiAIAsync(content);
        //if (!isSafe) throw new ValidationException("Comment contains inappropriate or harmful content.");

        var sanitized = SanitizeBadWords(content);
        return IsMeaninglessContent(sanitized)
            ? throw new ValidationException("Comment contains meaningless or spam-like content.")
            : sanitized;
    }

    private bool IsMeaninglessContent(string content)
    {
        var text = content.Trim().ToLower();

        if (AllowedShortWords.Contains(text)) return false;
        return text.All(c => char.IsPunctuation(c) || char.IsSymbol(c)) || HasSuspiciousRepetition(text);
    }

    private string SanitizeBadWords(string content)
    {
        return Regex.Replace(content, @"\p{L}+", match =>
        {
            var word = match.Value;
            var normalized = RemoveDiacritics(word.ToLower());
            return BadWords.Contains(normalized) ? "***" : word;
        });
    }

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in from c in normalized
                 let unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
                 where unicodeCategory != UnicodeCategory.NonSpacingMark
                 select c) sb.Append(c);

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static bool HasSuspiciousRepetition(string text)
    {
        return (text.Distinct().Count() == 1 && text.Length >= 3) || Regex.IsMatch(text, @"(.)\1{4,}");
    }
}