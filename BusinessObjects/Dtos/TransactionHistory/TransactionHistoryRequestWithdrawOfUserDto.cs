using BusinessObjects.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.TransactionHistory
{
    public class TransactionHistoryRequestWithdrawOfUserDto
    {
        public string Id { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public int Amount { get; set; }
        public string WalletId { get; set; }
        public string TransactionCode { get; set; }
        public DateTime DataTime { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AccountBankName { get; set; }
        public string BankNumber { get; set; }
        public string BankId { get; set; }
    }
}
