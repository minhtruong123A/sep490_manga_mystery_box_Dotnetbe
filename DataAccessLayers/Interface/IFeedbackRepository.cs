using BusinessObjects;
using BusinessObjects.Dtos.Feedback;

namespace DataAccessLayers.Interface;

public interface IFeedbackRepository : IGenericRepository<Feedback>
{
    Task<List<FeedbackResponeDto>> GetAllFeedbackOfProductSaleAsync(string sellProductId);
    Task<bool> CheckExistFeedbackAsync(string userId);
}