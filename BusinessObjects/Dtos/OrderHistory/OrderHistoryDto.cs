﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.OrderHistory
{
    public class OrderHistoryDto
    {
        public string Type { get; set; }

        public string BoxId { get; set; }
        public string BoxName { get; set; }

        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }
        public int TotalAmount { get; set; }
        public string TransactionCode { get; set; }
        public DateTime PurchasedAt { get; set; }
    }
}
