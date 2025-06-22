using BusinessObjects.Dtos.TransactionHistory;
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

        public async Task<List<TransactionHistoryDto>> GetTransactionHistoryAsync(string walletId)
        {
            return await _unitOfWork.TransactionHistoryRepository.GetTransactionsByWalletIdAsync(walletId);
        }
    }
}
