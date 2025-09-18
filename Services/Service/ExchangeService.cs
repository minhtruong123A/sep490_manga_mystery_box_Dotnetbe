using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Exchange;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class ExchangeService(IExchangeRepository repo, IMapper mapper) : IExchangeService
{
    private readonly IMapper _mapper = mapper;

    public async Task<ExchangeInfo> CreateExchangeAsync(ExchangeInfo info, List<ExchangeProduct> products,
        ExchangeSession session)
    {
        return await repo.CreateExchangeAsync(info, products, session);
    }

    public async Task<bool> AcceptExchangeAsync(string exchangeId, string currentUserId)
    {
        return await repo.AcceptExchangeAsync(exchangeId, currentUserId);
    }

    public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsByItemReciveIdAsync(string userId)
    {
        return await repo.GetExchangesWithProductsByItemReciveIdAsync(userId);
    }

    public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsOfBuyerAsync(string userId)
    {
        return await repo.GetExchangesWithProductsOfBuyerAsync(userId);
    }

    public async Task<bool> CancelExchangeAsync(string exchangeId, string userId)
    {
        return await repo.CancelExchangeAsync(exchangeId, userId);
    }

    public async Task<bool> RejectExchangeAsync(string exchangeId, string userId)
    {
        return await repo.RejectExchangeAsync(exchangeId, userId);
    }
}