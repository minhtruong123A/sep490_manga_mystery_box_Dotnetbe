using BusinessObjects;
using BusinessObjects.Dtos.Exchange;

namespace Services.Interface;

public interface IExchangeService
{
    Task<ExchangeInfo> CreateExchangeAsync(ExchangeInfo info, List<ExchangeProduct> products, ExchangeSession session);
    Task<bool> AcceptExchangeAsync(string exchangeId, string currentUserId);
    Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsByItemReciveIdAsync(string userId);
    Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsOfBuyerAsync(string userId);
    Task<bool> RejectExchangeAsync(string exchangeId, string userId);
    Task<bool> CancelExchangeAsync(string exchangeId, string userId);
}