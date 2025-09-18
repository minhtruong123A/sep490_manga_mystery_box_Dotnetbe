using BusinessObjects.Dtos.Feedback;

namespace Services.Interface;

public interface IFeedbackService
{
    Task<bool> CreateFeedbackAsync(string userId, FeedbackCreateDto dto);
    Task<List<FeedbackResponeDto>> GetAllFeedbackOfProductSaleAsync(string sellProductId);
}