﻿using BusinessObjects;
using BusinessObjects.Dtos.Comment;
using DataAccessLayers.Interface;
using DnsClient;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Service
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        private readonly IModerationService _moderationService;
        private static readonly HashSet<string> BadWords = LoadBadWords();
        private static readonly HashSet<string> AllowedShortWords = LoadAllowedShortWords();

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

        public CommentService(IUnitOfWork unitOfWork, IModerationService moderationService)
        {
            _uniUnitOfWork = unitOfWork;
            _moderationService = moderationService;
        }

        public async Task<List<CommentWithUsernameDto>> GetAlCommentlBySellProductIdAsync(string sellProductId) => await _uniUnitOfWork.CommentRepository.GetAlCommentlBySellProductIdAsync(sellProductId);
        public async Task<List<RatingWithUsernameDto>> GetAllRatingBySellProductIdAsync(string sellProductId) => await _uniUnitOfWork.CommentRepository.GetAllRatingBySellProductIdAsync(sellProductId);

        //rating create and validation
        public async Task<Comment> CreateRatingOnlyAsync(string sellProductId, string userId, float rating)
        {
            if (string.IsNullOrWhiteSpace(sellProductId)) throw new Exception("SellProductId must not be empty or whitespace.");
            
            if (rating < 0 || rating > 5) throw new Exception("Rating must be between 0 and 5.");

            var product = await _uniUnitOfWork.SellProductRepository.GetByIdAsync(sellProductId);
            if (product == null || !product.IsSell) throw new Exception("Product not found or not available for sale.");

            var existingRatingOnly = await _uniUnitOfWork.CommentRepository.GetRatingOnlyByUserAndProductAsync(userId, sellProductId);
            if (existingRatingOnly != null)
            {
                existingRatingOnly.Rating = rating;
                existingRatingOnly.UpdatedAt = DateTime.UtcNow;
                await _uniUnitOfWork.CommentRepository.UpdateAsync(existingRatingOnly.Id, existingRatingOnly);
                return existingRatingOnly;
            }

            return await _uniUnitOfWork.CommentRepository.CreateRatingOnlyAsync(sellProductId, userId, rating);
        }

        //comment create and validation
        public async Task<Comment> CreateCommentAsync(string sellProductId, string userId, string content)
        {
            var sanitizedContent = ValidateCommentInput(sellProductId, content);

            var product = await _uniUnitOfWork.SellProductRepository.GetByIdAsync(sellProductId);
            if (product == null || !product.IsSell) throw new Exception("Product not found or not available for sale.");

            var existingCommentOnly = await _uniUnitOfWork.CommentRepository.GetCommentOnlyByUserAndProductAsync(userId, sellProductId);
            if (existingCommentOnly != null)
            {
                existingCommentOnly.Content = sanitizedContent;
                existingCommentOnly.UpdatedAt = DateTime.UtcNow;
                await _uniUnitOfWork.CommentRepository.UpdateAsync(existingCommentOnly.Id, existingCommentOnly);
                return existingCommentOnly;
            }

            return await _uniUnitOfWork.CommentRepository.CreateCommentAsync(sellProductId, userId, sanitizedContent);
        }
        private string ValidateCommentInput(string sellProductId, string content)
        {
            if (string.IsNullOrWhiteSpace(sellProductId)) throw new Exception("SellProductId must not be empty.");
            if (string.IsNullOrWhiteSpace(content)) throw new Exception("Comment content cannot be empty.");
            if (content.Length > 1000) throw new Exception("Comment content too long (max 1000 characters).");
            //bool isSafe = await _moderationService.IsContentSafeGeminiAIAsync(content);
            //if (!isSafe) throw new ValidationException("Comment contains inappropriate or harmful content.");

            var sanitized = SanitizeBadWords(content);
            if (IsMeaninglessContent(sanitized)) throw new ValidationException("Comment contains meaningless or spam-like content.");
            return sanitized;

        }
        private bool IsMeaninglessContent(string content)
        {
            var text = content.Trim().ToLower();

            if (AllowedShortWords.Contains(text)) return false;
            if (text.All(c => char.IsPunctuation(c) || char.IsSymbol(c))) return true;
            if (HasSuspiciousRepetition(text)) return true;

            return false;
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
        public static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        private static bool HasSuspiciousRepetition(string text) => (text.Distinct().Count() == 1 && text.Length >= 3) || Regex.IsMatch(text, @"(.)\1{4,}");

        // delete all comment
        public async Task DeleteAllCommentAsync() => await _uniUnitOfWork.CommentRepository.DeleteAllAsync();

        // get all bad words
        public Task<List<string>> GetAllBadWordsAsync()
        {
            return Task.FromResult(BadWords.ToList());
        }

        //public Task<List<string>> GetAllAllowedShortWordsAsync()
        //{
        //    return Task.FromResult(AllowedShortWords.ToList());
        //}
    }
}