using BusinessObjects;
using BusinessObjects.Dtos.Comment;

namespace DataAccessLayers.Interface;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<Comment> CreateCommentAsync(string sellProductId, string userId, string content);
    Task<List<CommentWithUsernameDto>> GetAlCommentlBySellProductIdAsync(string sellProductId);
    Task<Comment> CreateRatingOnlyAsync(string sellProductId, string userId, float rating);
    Task<List<RatingWithUsernameDto>> GetAllRatingBySellProductIdAsync(string sellProductId);
    Task<Comment?> GetRatingOnlyByUserAndProductAsync(string userId, string productId);
    Task<Comment?> GetCommentOnlyByUserAndProductAsync(string userId, string productId);
    Task<List<CommentWithUsernameDto>> GetAllCommentProductOfUserAsync(string userId, string productName);
    Task<float> GetRatingOfUserAsync(string userId);
    Task<float> GetTotalAverageOfSellProductByIdAsync(string sellProductId);
}