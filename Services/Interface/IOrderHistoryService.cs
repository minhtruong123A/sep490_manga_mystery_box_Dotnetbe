using BusinessObjects.Dtos.OrderHistory;

namespace Services.Interface;

public interface IOrderHistoryService
{
    Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string userId);
    Task<List<UserOrderHistoryResultDto>> GetAllUserOrderHistoriesAsync();
    Task<OrderHistoryDto?> GetOrderHistoryByIdAsync(string orderHistoryId);
}