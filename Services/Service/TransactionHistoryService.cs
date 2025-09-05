using BusinessObjects;
using BusinessObjects.Dtos.TransactionHistory;
using BusinessObjects.Enum;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Options;
using Services.Interface;

namespace Services.Service;

public class TransactionHistoryService(IUnitOfWork unitOfWork, IOptions<WithdrawRulesSettings> withdrawRulesSettings)
    : ITransactionHistoryService
{
    private readonly WithdrawRulesSettings _withdrawRulesSettings = withdrawRulesSettings.Value;

    public async Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(string walletId)
    {
        return await unitOfWork.TransactionHistoryRepository.GetTransactionsByWalletIdAsync(walletId);
    }

    public async Task<object?> CreateRequestWithdrawAsync(string userId, int amount)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var userBanks = await unitOfWork.UserBankRepository.GetAllAsync();
        var userBank = userBanks.FirstOrDefault(x => x.UserId.Equals(user.Id));
        if (userBank == null) return null;

        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1).AddTicks(-1);
        var withdrawCountTodayLong = await unitOfWork.TransactionHistoryRepository.CountAsync(x =>
            x.WalletId == user.WalletId &&
            x.Type == (int)TransactionType.Withdraw &&
            x.DataTime >= todayStart &&
            x.DataTime <= todayEnd);
        var withdrawCountToday = (int)withdrawCountTodayLong;
        var remainingLimit = Math.Max(0, _withdrawRulesSettings.LimitWithdraw - withdrawCountToday);

        if (amount < _withdrawRulesSettings.MinAmount || amount > _withdrawRulesSettings.MaxAmount)
            return new WithdrawLimitInfoDto
            {
                WithdrawCountToday = withdrawCountToday,
                RemainingLimit = remainingLimit,
                Message =
                    $"The withdrawal amount must be between {_withdrawRulesSettings.MinAmount:N0} VND and {_withdrawRulesSettings.MaxAmount:N0} VND."
            };

        if (withdrawCountToday >= _withdrawRulesSettings.LimitWithdraw)
            return new WithdrawLimitInfoDto
            {
                WithdrawCountToday = withdrawCountToday,
                RemainingLimit = remainingLimit,
                Message =
                    $"You have reached the maximum number of withdraw requests for today ({_withdrawRulesSettings.LimitWithdraw})."
            };

        var newWithdraw = new TransactionHistory
        {
            Amount = amount,
            WalletId = user.WalletId,
            Type = (int)TransactionType.Withdraw,
            Status = (int)TransactionStatus.Pending,
            DataTime = DateTime.UtcNow,
            TransactionCode = _withdrawRulesSettings.Statuses.WaitingModReview
        };
        await unitOfWork.TransactionHistoryRepository.AddAsync(newWithdraw);
        await unitOfWork.SaveChangesAsync();

        return new TransactionHistoryDto
        {
            Id = newWithdraw.Id,
            Amount = newWithdraw.Amount,
            Status = (TransactionStatus)newWithdraw.Status,
            Type = (TransactionType)newWithdraw.Type,
            DataTime = newWithdraw.DataTime,
            TransactionCode = newWithdraw.TransactionCode
        };
    }

    public async Task<List<UserTransactionDto>> GetAllUsersWithTransactionsAsync()
    {
        var users = await unitOfWork.UserRepository.GetAllAsync();
        var result = new List<UserTransactionDto>();

        foreach (var user in users)
            if (!string.IsNullOrEmpty(user.WalletId))
            {
                var transactions =
                    await unitOfWork.TransactionHistoryRepository.GetTransactionsByWalletIdAsync(user.WalletId);

                result.Add(new UserTransactionDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    WalletId = user.WalletId,
                    Transactions = transactions
                });
            }

        return result;
    }

    public async Task<List<TransactionHistoryRequestWithdrawOfUserDto>> GetAllRequestWithdrawAsync()
    {
        return await unitOfWork.TransactionHistoryRepository.GetAllRequestWithdrawAsync();
    }

    public async Task<bool> AcceptTransactionWithdrawAsync(string transactionId, string transactionCode)
    {
        var transaction = await unitOfWork.TransactionHistoryRepository.GetByIdAsync(transactionId);
        if (transaction == null) throw new Exception("Transaction not found");
        var wallet = await unitOfWork.UseDigitalWalletRepository.GetByIdAsync(transaction.WalletId);
        if (wallet == null) throw new Exception("Wallet not found");
        if (wallet.Ammount < transaction.Amount) throw new Exception("User wallet not enough");
        wallet.Ammount -= transaction.Amount;
        await unitOfWork.UseDigitalWalletRepository.UpdateAsync(transaction.WalletId, wallet);

        transaction.TransactionCode = transactionCode;
        transaction.Status = (int)TransactionStatus.Success;
        await unitOfWork.TransactionHistoryRepository.UpdateAsync(transactionId, transaction);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectTransactionWithdrawAsync(string transactionId)
    {
        var transaction = await unitOfWork.TransactionHistoryRepository.GetByIdAsync(transactionId);
        if (transaction == null) throw new Exception("Transaction not found");

        transaction.Status = (int)TransactionStatus.Cancel;
        transaction.TransactionCode = _withdrawRulesSettings.Statuses.RejectedByMod;
        await unitOfWork.TransactionHistoryRepository.UpdateAsync(transactionId, transaction);
        await unitOfWork.SaveChangesAsync();
        return true;
    }
}