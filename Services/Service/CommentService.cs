using BusinessObjects;
using BusinessObjects.Dtos.Comment;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task<Comment> CreateRatingOnlyAsync(string sellProductId, string userId, float rating)
        {
            if (string.IsNullOrWhiteSpace(sellProductId)) throw new Exception("SellProductId must not be empty or whitespace.");
            if (rating < 0) throw new Exception("Rating must be greater than or equal to 0.");
            return await _uniUnitOfWork.CommentRepository.CreateRatingOnlyAsync(sellProductId, userId, rating);
        }
        public async Task<Comment> CreateCommentAsync(string sellProductId, string userId, string content)
        {
            if (string.IsNullOrWhiteSpace(sellProductId)) throw new Exception("SellProductId must not be empty or whitespace.");
            if (string.IsNullOrWhiteSpace(content)) throw new Exception("Comment content cannot be empty.");
            return await _uniUnitOfWork.CommentRepository.CreateCommentAsync(sellProductId, userId, content);
        }

        public async Task DeleteAllCommentAsync() => await _uniUnitOfWork.CommentRepository.DeleteAllAsync();

    }
}
