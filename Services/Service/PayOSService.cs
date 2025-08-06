using BusinessObjects;
using BusinessObjects.Dtos.PayOS;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Net.payOS;
using Net.payOS.Types;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Service
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _payOS;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMongoClient _mongoClient;

        public PayOSService(
            IOptions<PayOSConfig> configOptions,
            IUnitOfWork unitOfWork,
            IMongoClient mongoClient)
        {
            var config = configOptions.Value ?? throw new ArgumentNullException(nameof(configOptions));

            if (string.IsNullOrWhiteSpace(config.ClientId) ||
                string.IsNullOrWhiteSpace(config.ApiKey) ||
                string.IsNullOrWhiteSpace(config.ChecksumKey))
            {
                throw new ArgumentException("PayOS configuration is incomplete.");
            }

            Console.WriteLine($"[DEBUG] ClientId: {config.ClientId}");
            Console.WriteLine($"[DEBUG] ApiKey Prefix: {config.ApiKey.Substring(0, 8)}***");
            Console.WriteLine($"[DEBUG] ChecksumKey Prefix: {config.ChecksumKey.Substring(0, 8)}***");

            _payOS = new PayOS(config.ClientId, config.ApiKey, config.ChecksumKey);
            _unitOfWork = unitOfWork;
            _mongoClient = mongoClient;
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkAsync(
            long orderCode,
            int amount,
            string description,
            List<ItemData> items,
            string userId)
        {
            var expiredAt = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds(); 
            var paymentData = new PaymentData(
                orderCode,
                amount,
                description,
                items,
                cancelUrl: "https://youtu.be/b3rNUhDqciM?si=mryd9IU_aOcrW82s",
                returnUrl: "https://youtu.be/dQw4w9WgXcQ?si=5BxIvepyCzYv25hf",
                expiredAt: expiredAt
            );

            var paymentResult = await _payOS.createPaymentLink(paymentData);

            using var session = await _mongoClient.StartSessionAsync();
            session.StartTransaction();
            try
            {
                var user = await _unitOfWork.UserRepository.FindOneAsync(session, x => x.Id == userId)
                           ?? throw new Exception("User not found");

                var walletId = user.WalletId;

                if (string.IsNullOrEmpty(walletId))
                {
                    var newWallet = new UseDigitalWallet { Ammount = 0, IsActive = true };
                    await _unitOfWork.UseDigitalWalletRepository.AddAsync(session, newWallet);
                    walletId = newWallet.Id;

                    var updateUser = Builders<User>.Update.Set(x => x.WalletId, walletId);
                    await _unitOfWork.UserRepository.UpdateFieldAsync(session, x => x.Id == user.Id, updateUser);
                }

                var transaction = new TransactionHistory
                {
                    WalletId = walletId,
                    DataTime = DateTime.UtcNow,
                    Type = 1, // Nạp tiền
                    Status = 1, // Chờ thanh toán
                    Amount = amount,
                    TransactionCode = orderCode.ToString()
                };

                await _unitOfWork.TransactionHistoryRepository.AddAsync(session, transaction);
                await session.CommitTransactionAsync();

                return paymentResult;
            }
            catch
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        public async Task<bool> ProcessRechargeAsync(string orderCode, int amount)
            => await _unitOfWork.PayOSRepository.RechargeWalletAsync(orderCode, amount);

        public async Task<int> CheckAndUpdatePendingTransactionsAsync()
        {
            var pendingTransactions = await _unitOfWork.TransactionHistoryRepository.FilterByAsync(x => x.Status == 1);
            int updatedCount = 0;

            foreach (var tx in pendingTransactions)
            {
                var status = await GetPayOSStatusViaSdkAsync(tx.TransactionCode);
                Console.WriteLine($"[DEBUG] OrderCode: {tx.TransactionCode}, Status: {status}");

                if (status == "PAID")
                {
                    await ProcessRechargeAsync(tx.TransactionCode, tx.Amount);
                    updatedCount++;
                }
                else if (status == "EXPIRED" || status == "CANCELLED")
                {
                    var update = Builders<TransactionHistory>.Update.Set(x => x.Status, 3);
                    await _unitOfWork.TransactionHistoryRepository.UpdateFieldAsync(x => x.Id == tx.Id, update);
                    updatedCount++;
                }
            }

            return updatedCount;
        }

        private async Task<string> GetPayOSStatusViaSdkAsync(string orderCode)
        {
            try
            {
                if (!long.TryParse(orderCode, out var orderCodeLong))
                {
                    Console.WriteLine($"[WARN] Invalid orderCode: {orderCode}");
                    return "UNKNOWN";
                }

                var result = await _payOS.getPaymentLinkInformation(orderCodeLong);
                return result?.status ?? "UNKNOWN";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to fetch status from PayOS SDK: {ex.Message}");
                return "UNKNOWN";
            }
        }

        public async Task<bool> HasOrderBeenProcessedAsync(string orderCode)
        {
            var tx = await _unitOfWork.TransactionHistoryRepository
                .FindOneAsync(x => x.TransactionCode == orderCode && x.Status == 2);

            return tx != null;
        }
    }
}
