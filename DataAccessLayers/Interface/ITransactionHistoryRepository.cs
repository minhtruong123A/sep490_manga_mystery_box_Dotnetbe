using BusinessObjects;
using BusinessObjects.Dtos.TransactionHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface ITransactionHistoryRepository : IGenericRepository<TransactionHistory>
    {
        Task<List<TransactionHistoryDto>> GetTransactionsByWalletIdAsync(string walletId);
    }
}
