using BusinessObjects;
using BusinessObjects.Dtos.Exchange;

namespace DataAccessLayers.Interface;

public interface IExchangeRepository : IGenericRepository<ExchangeInfo>
{
    Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsByItemReciveIdAsync(string userId);
    Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsOfBuyerAsync(string userId);
    Task<ExchangeInfo> CreateExchangeAsync(ExchangeInfo info, List<ExchangeProduct> products, ExchangeSession session);
    Task<bool> AcceptExchangeAsync(string exchangeId, string currentUserId);
    Task<ExchangeInfo> GetExchangeInfoById(string id);
    Task<bool> CancelExchangeAsync(string exchangeId, string userId);
    Task<bool> RejectExchangeAsync(string exchangeId, string userId);
    Task<bool> RejectExchangeAutoWhenCancelSellProductAsync(string sellProductId);
}