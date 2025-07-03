using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enum;

namespace BusinessObjects.Dtos.TransactionHistory
{
    public class TransactionHistoryDto
    {
        public string Id { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public int Amount { get; set; }
        public string TransactionCode { get; set; }
        public DateTime DataTime { get; set; }
    }
}
