using BusinessObjects;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using Services.Interface;

namespace Services.Service;

public class UseDigitalWalletService(IUnitOfWork unitOfWork) : IUseDigitalWalletService
{
    public async Task<UseDigitalWallet?> GetWalletByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out var objectId)) return null;

        return await unitOfWork.UseDigitalWalletRepository.GetOneAsync(c => c.Id == id);
    }

    public async Task<string> UpdateWalletWithTransactionAsync(string userId, int amount)
    {
        var user = await unitOfWork.UserRepository.FindOneAsync(x => x.Id == userId) ??
                   throw new Exception("Account not existed in system!");
        var walletId = user.WalletId;
        var wallet = new UseDigitalWallet();
        if (string.IsNullOrEmpty(walletId))
        {
            wallet = new UseDigitalWallet
            {
                Ammount = amount,
                IsActive = true
            };
            await unitOfWork.UseDigitalWalletRepository.AddAsync(wallet);
            walletId = wallet.Id;
            var updateUser = Builders<User>.Update.Set(x => x.WalletId, walletId);
            await unitOfWork.UserRepository.UpdateFieldAsync(x => x.Id == user.Id, updateUser);
        }
        else
        {
            wallet = await unitOfWork.UseDigitalWalletRepository.FindOneAsync(x => x.Id == walletId);
            if (wallet == null) throw new Exception("Wallet not found");
            wallet.Ammount += amount;
            await unitOfWork.UseDigitalWalletRepository.UpdateAsync(wallet.Id, wallet);
        }

        var transaction = new TransactionHistory
        {
            WalletId = walletId,
            DataTime = DateTime.UtcNow,
            Type = 1,
            Status = 2, // 2 is success, 1 is pending
            Amount = amount,
            TransactionCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
        };
        await unitOfWork.TransactionHistoryRepository.AddAsync(transaction);
        await unitOfWork.SaveChangesAsync();

        return transaction.TransactionCode;
    }
}