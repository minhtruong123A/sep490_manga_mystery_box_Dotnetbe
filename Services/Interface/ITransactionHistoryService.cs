using BusinessObjects.Dtos.TransactionHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ITransactionHistoryService
    {
        Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(string walletId);
        Task<List<TransactionHistoryRequestWithdrawOfUserDto>> GetAllRequestWithdrawAsync();
        Task<object?> CreateRequestWithdrawAsync(string userId, int amount);
        Task<bool> AcceptTransactionWithdrawAsync(string transactionId, string transactionCode);
        Task<bool> RejectTransactionWithdrawAsync(string transactionId);

    }
}
