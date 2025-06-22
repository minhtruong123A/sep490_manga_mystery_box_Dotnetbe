using BusinessObjects;
using BusinessObjects.Dtos.PayOS;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Net.payOS;
using Net.payOS.Types;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _payOS;
        private readonly IUnitOfWork _unitOfWork;

        public PayOSService(IOptions<PayOSConfig> config, IUnitOfWork unitOfWork)
        {
            var cfg = config.Value;
            _payOS = new PayOS(cfg.ClientId, cfg.ApiKey, cfg.ChecksumKey);
            _unitOfWork = unitOfWork;
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkAsync(
                long orderCode,
                int amount,
                string description,
                List<ItemData> items,
                string userId
        )
        {
            var paymentData = new PaymentData(
                orderCode,
                amount,
                description,
                items,
                cancelUrl: "https://yourfrontend.com/cancel", // dang ky trong trang nap
                returnUrl: "https://yourfrontend.com/success" // dang ky trong trang nap
            );
            var paymentResult = await _payOS.createPaymentLink(paymentData);

            var user = await _unitOfWork.UserRepository.FindOneAsync(x => x.Id == userId);
            if (user == null) throw new Exception("Account not existed in system!");
            string walletId = user.WalletId;
            if (string.IsNullOrEmpty(user.WalletId))
            {
                var newWallet = new UseDigitalWallet
                {
                    Ammount = 0,
                    IsActive = true
                };
                await _unitOfWork.UseDigitalWalletRepository.AddAsync(newWallet);
                walletId = newWallet.Id;
                var updateUser = Builders<User>.Update.Set(x => x.WalletId, walletId);
                await _unitOfWork.UserRepository.UpdateFieldAsync(x => x.Id == user.Id, updateUser);
            }
            var transaction = new TransactionHistory
            {
                WalletId = walletId,
                DataTime = DateTime.UtcNow,
                Type = 1,
                Status = 1,
                Amount = amount,
                TransactionCode = orderCode.ToString()
            };

            await _unitOfWork.TransactionHistoryRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return paymentResult;
        }

        public async Task<bool> ProcessRechargeAsync(string orderCode, int amount) => await _unitOfWork.PayOSRepository.RechargeWalletAsync(orderCode, amount);
    }
}
