using BusinessObjects;
using BusinessObjects.Dtos.TransactionHistory;

namespace DataAccessLayers.Interface;

public interface ITransactionHistoryRepository : IGenericRepository<TransactionHistory>
{
    Task<List<TransactionHistoryDto>> GetTransactionsByWalletIdAsync(string walletId);
    Task<List<TransactionHistoryRequestWithdrawOfUserDto>> GetAllRequestWithdrawAsync();
}