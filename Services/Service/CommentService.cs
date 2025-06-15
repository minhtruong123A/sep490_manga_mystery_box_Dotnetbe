using BusinessObjects;
using BusinessObjects.Dtos.Comment;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Service
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        public CommentService(IUnitOfWork unitOfWork)
        {
            _uniUnitOfWork = unitOfWork;
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
            ValidateCommentInput(sellProductId, content);
            
            var product = await _uniUnitOfWork.SellProductRepository.GetByIdAsync(sellProductId);
            if (product == null || !product.IsSell) throw new Exception("Product not found or not available for sale.");

            return await _uniUnitOfWork.CommentRepository.CreateCommentAsync(sellProductId, userId, content);
        }

        private void ValidateCommentInput(string sellProductId, string content)
        {
            if (string.IsNullOrWhiteSpace(sellProductId)) throw new Exception("SellProductId must not be empty.");
            if (string.IsNullOrWhiteSpace(content)) throw new Exception("Comment content cannot be empty.");
            if (content.Length > 1000) throw new Exception("Comment content too long (max 1000 characters).");
            if (IsMeaninglessContent(content)) throw new ValidationException("Comment content must contain meaningful words.");
        }

        private bool IsMeaninglessContent(string content)
        {
            var text = content.Trim().ToLower();

            if (AllowedShortWords.Contains(text)) return false;
            if (text.All(c => char.IsPunctuation(c) || char.IsSymbol(c))) return true;
            if (HasSuspiciousRepetition(text)) return true;
            if (text.Count(char.IsLetterOrDigit) < 3) return true;
            if (IsTooShortAndUnknown(text)) return true;

            return false;
        }

        private bool IsTooShortAndUnknown(string text)
        {
            var words = Regex.Split(text, @"\W+").Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
            if (words.Length == 0) return true;
            if (words.Length < 5)
            {
                int unknownCount = words.Count(w => !BasicWords.Contains(w.ToLower()));
                return (double)unknownCount / words.Length > 0.8;
            }
            if (words.Length >= 5)
            {
                int unknownCount = words.Count(w => !BasicWords.Contains(w.ToLower()));
                return (double)unknownCount / words.Length > 0.98;
            }
            if (IsLowVowelRatio(text) && IsMostlyUnknownWords(text)) return true;

            return false;
        }

        private static readonly HashSet<string> AllowedShortWords = new() {"ok", "ổn", "ừ", "ờ", "tốt", "no", "yes", "ko", "k", "có", "hmm", "haha"};

        private static readonly HashSet<string> BasicWords = new()
        {
            // Vietnamese
            "tốt", "ok", "sản", "phẩm", "đẹp", "giao", "hàng", "nhanh", "quá", "ổn", "xấu",
            "chậm", "thật", "sự", "hài", "lòng", "dùng", "ưng", "rẻ", "giá", "bền", "rất", "liền",
            "xịn", "chất", "mượt", "êm", "mạnh", "ổn áp", "mượt mà", "bền bỉ", "sắc nét", "xài", "ngon", "xứng", "đáng",
            "tuyệt", "vời", "đáng", "tiền", "tốt thật", "hàng chuẩn", "hàng xịn", "đúng mô tả", "đúng hàng", "hợp lý",
            "đẹp lắm", "ưng lắm", "xài ổn", "quá ngon", "đáng mua", "đẹp", "dỏm", "này", "vượt", "ngoài", "mong", "đợi", "chất", "lượng", 
            "nhanh chóng", "đóng", "gói", "cẩn thận", "ủng hộ", "lần nữa", "regrets", "vibes", "cat", "moon", "potato", "teacher", "box", "broke", 
            "guess", "thing", "3am", "documentary", "funny", "coincidence",


            // English
            "nice", "cool", "great", "bad", "awesome", "terrible", "excellent", "boring", "cheap", "expensive",
            "worthless", "amazing", "fast", "slow", "love", "hate", "good", "clean", "dirty", "ugly", "perfect",
            "fine", "yummy", "tasty", "very", "so", "it", "product", "quality", "beautiful", "smooth", "strong",
            "sharp", "worth", "price", "value", "super", "recommend", "satisfied", "happy", "okay", "decent"
        };

        private static bool HasSuspiciousRepetition(string text) => (text.Distinct().Count() == 1 && text.Length >= 3) || Regex.IsMatch(text, @"(.)\1{4,}");

        private static bool IsLowVowelRatio(string text)
        {
            var letters = text.Where(char.IsLetter).ToList();
            if (letters.Count < 3) return true;

            double vowelRatio = letters.Count(c => "aeiouy".Contains(c)) / (double)letters.Count;
            return vowelRatio < 0.15;
        }

        private static bool IsMostlyUnknownWords(string text)
        {
            var words = Regex.Split(text, @"\W+");
            if (words.Length == 0) return true;

            int unknownCount = words.Count(w => !BasicWords.Contains(w.ToLower()));
            return (double)unknownCount / words.Length >= 0.98;
        }


        // delete all comment
        public async Task DeleteAllCommentAsync() => await _uniUnitOfWork.CommentRepository.DeleteAllAsync();
    }
}