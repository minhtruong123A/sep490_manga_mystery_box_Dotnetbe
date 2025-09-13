using BusinessObjects.Dtos.TransactionHistory;

namespace Services.Interface;

public interface ITransactionHistoryService
{
    Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(string walletId);
    Task<List<TransactionHistoryRequestWithdrawOfUserDto>> GetAllRequestWithdrawAsync();
    Task<object?> CreateRequestWithdrawAsync(string userId, int amount);
    Task<bool> AcceptTransactionWithdrawAsync(string transactionId, string transactionCode);
    Task<bool> RejectTransactionWithdrawAsync(string transactionId);
    Task<List<UserTransactionDto>> GetAllUsersWithTransactionsAsync();
}