﻿using BusinessObjects.Dtos.OrderHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IOrderHistoryService
    {
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string userId);
    }
}
