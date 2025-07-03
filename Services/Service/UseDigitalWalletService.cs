using BusinessObjects;
using DataAccessLayers.Interface;
using DataAccessLayers.UnitOfWork;
using MongoDB.Bson;
using MongoDB.Driver;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class UseDigitalWalletService : IUseDigitalWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UseDigitalWalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UseDigitalWallet?> GetWalletByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out var objectId)) return null;

            return await _unitOfWork.UseDigitalWalletRepository.GetOneAsync(c => c.Id == id);
        }

        public async Task<string> UpdateWalletWithTransactionAsync(string userId, int amount)
        {
            var user = await _unitOfWork.UserRepository.FindOneAsync(x => x.Id == userId) ?? throw new Exception("Account not existed in system!");
            string walletId = user.WalletId;
            var wallet = new UseDigitalWallet();
            if (string.IsNullOrEmpty(walletId))
            {
                wallet = new UseDigitalWallet
                {
                    Ammount = amount,
                    IsActive = true
                };
                await _unitOfWork.UseDigitalWalletRepository.AddAsync(wallet);
                walletId = wallet.Id;
                var updateUser = Builders<User>.Update.Set(x => x.WalletId, walletId);
                await _unitOfWork.UserRepository.UpdateFieldAsync(x => x.Id == user.Id, updateUser);
            }
            else
            {
                wallet = await _unitOfWork.UseDigitalWalletRepository.FindOneAsync(x => x.Id == walletId);
                if (wallet == null) throw new Exception("Wallet not found");
                wallet.Ammount += amount;
                await _unitOfWork.UseDigitalWalletRepository.UpdateAsync(wallet.Id, wallet);
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
            await _unitOfWork.TransactionHistoryRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return transaction.TransactionCode;
        }
    }
}
