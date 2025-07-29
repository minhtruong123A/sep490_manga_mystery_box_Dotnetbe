using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Exchange;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRepository _repo;
        private readonly IMapper _mapper;

        public ExchangeService(IExchangeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ExchangeInfo> CreateExchangeAsync(ExchangeInfo info, List<ExchangeProduct> products, ExchangeSession session) => await _repo.CreateExchangeAsync(info, products, session);
        public async Task<bool> AcceptExchangeAsync(string exchangeId, string currentUserId) => await _repo.AcceptExchangeAsync(exchangeId, currentUserId);
        public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsByItemReciveIdAsync(string userId) => await _repo.GetExchangesWithProductsByItemReciveIdAsync(userId);
        public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsOfBuyerAsync(string userId) => await _repo.GetExchangesWithProductsOfBuyerAsync(userId);
        public async Task<bool> CancelExchangeAsync(string exchangeId, string userId) => await _repo.CancelExchangeAsync(exchangeId,userId);
        public async Task<bool> RejectExchangeAsync(string exchangeId, string userId) => await _repo.RejectExchangeAsync(exchangeId,userId);
        

    }

}
