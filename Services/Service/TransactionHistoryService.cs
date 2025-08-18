using BusinessObjects.Dtos.TransactionHistory;
using BusinessObjects.Enum;
using BusinessObjects;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class TransactionHistoryService : ITransactionHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(string walletId) => await _unitOfWork.TransactionHistoryRepository.GetTransactionsByWalletIdAsync(walletId);
        public async Task<List<TransactionHistoryDto>> GetTransactionsWithdrawByWalletIdAsync(string walletId) => await _unitOfWork.TransactionHistoryRepository.GetTransactionsWithdrawByWalletIdAsync(walletId);
        public async Task<bool> CreateRequestWithdrawAsync(string userId,int amount)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");
            var userBanks = await _unitOfWork.UserBankRepository.GetAllAsync();
            var userBank = userBanks.FirstOrDefault(x=>x.UserId.Equals(user.Id));
            if (userBank == null) return false;

            var newWithdraw = new TransactionHistory
            {
                Amount = amount,
                WalletId = user.WalletId,
                Type = (int)TransactionType.Withdraw,
                Status = (int)TransactionStatus.Pending,
                DataTime = DateTime.UtcNow,
                TransactionCode = null
            };
            await _unitOfWork.TransactionHistoryRepository.AddAsync(newWithdraw);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<TransactionHistoryRequestWithdrawOfUserDto>> GetAllRequestWithdrawAsync() => await _unitOfWork.TransactionHistoryRepository.GetAllRequestWithdrawAsync();
        public async Task<bool> AcceptTransactionWithdrawAsync(string transactionId, string transactionCode)
        {
            var transaction = await _unitOfWork.TransactionHistoryRepository.GetByIdAsync(transactionId);
            if (transaction == null) throw new Exception("Transaction not found");
            var wallet = await _unitOfWork.UseDigitalWalletRepository.GetByIdAsync(transaction.WalletId);
            if (wallet == null) throw new Exception("Wallet not found");
            if (wallet.Ammount < transaction.Amount) throw new Exception("User wallet not enough");
            wallet.Ammount -= transaction.Amount;
            await _unitOfWork.UseDigitalWalletRepository.UpdateAsync(transaction.WalletId,wallet);

            transaction.TransactionCode = transactionCode;
            transaction.Status = (int)TransactionStatus.Success;
            await _unitOfWork.TransactionHistoryRepository.UpdateAsync(transactionId, transaction);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectTransactionWithdrawAsync(string transactionId)
        {
            var transaction = await _unitOfWork.TransactionHistoryRepository.GetByIdAsync(transactionId);
            if (transaction == null) throw new Exception("Transaction not found");

            transaction.Status = (int)TransactionStatus.Cancel;
            await _unitOfWork.TransactionHistoryRepository.UpdateAsync(transactionId, transaction);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
