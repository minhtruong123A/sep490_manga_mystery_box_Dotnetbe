﻿using BusinessObjects;
using BusinessObjects.Dtos.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IExchangeService
    {
        Task<ExchangeInfo> CreateExchangeAsync(ExchangeInfo info, List<ExchangeProduct> products, ExchangeSession session);
        Task<bool> AcceptExchangeAsync(string exchangeId);
        Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsByItemReciveIdAsync(string userId);
        Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsOfBuyerAsync(string userId);
        Task<bool> RejectExchangeAsync(string exchangeId, string userId);
        Task<bool> CancelExchangeAsync(string exchangeId, string userId);
    }

}
