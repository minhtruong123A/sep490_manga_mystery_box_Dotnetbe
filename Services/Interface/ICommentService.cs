using BusinessObjects;
using BusinessObjects.Dtos.Comment;

namespace Services.Interface;

public interface ICommentService
{
    Task<Comment> CreateCommentAsync(string sellProductId, string userId, string content);
    Task<List<CommentWithUsernameDto>> GetAlCommentlBySellProductIdAsync(string sellProductId);
    Task<Comment> CreateRatingOnlyAsync(string sellProductId, string userId, float rating);
    Task<List<RatingWithUsernameDto>> GetAllRatingBySellProductIdAsync(string sellProductId);
    Task DeleteAllCommentAsync();
    Task<List<string>> GetAllBadWordsAsync();
    Task<List<CommentWithUsernameDto>> GetAllCommentProductOfUserAsync(string userId, string productName);
    Task<float> GetRatingOfUser(string userId);

    Task<float> GetTotalAverageOfSellProductByIdAsync(string sellProductId);
    //Task<List<string>> GetAllAllowedShortWordsAsync();
}