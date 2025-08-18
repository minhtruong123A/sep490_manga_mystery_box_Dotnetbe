using BusinessObjects;
using BusinessObjects.Dtos.TransactionHistory;
using BusinessObjects.Enum;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class TransactionHistoryRepository : GenericRepository<TransactionHistory>, ITransactionHistoryRepository
    {
        private readonly IMongoCollection<TransactionHistory> _transactionCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<UserBank> _userBankCollection;
        public TransactionHistoryRepository(MongoDbContext context) : base(context.GetCollection<TransactionHistory>("TransactionHistory"))
        {
            _transactionCollection = context.GetCollection<TransactionHistory>("TransactionHistory");
            _userCollection = context.GetCollection<User>("User");
            _userBankCollection = context.GetCollection<UserBank>("UserBank");
        }

        public async Task<List<TransactionHistoryDto>> GetTransactionsByWalletIdAsync(string walletId)
        {
            //var filter = Builders<TransactionHistory>.Filter.And(
            //      Builders<TransactionHistory>.Filter.Eq(t => t.WalletId, walletId),
            //      Builders<TransactionHistory>.Filter.Eq(t => t.Type, (int)TransactionType.Recharge)
            //  ); 
            var filter = Builders<TransactionHistory>.Filter.And(
                Builders<TransactionHistory>.Filter.Eq(t => t.WalletId, walletId),
                Builders<TransactionHistory>.Filter.In(t => t.Type, new[] { (int)TransactionType.Recharge, (int)TransactionType.Withdraw })
            );
            var transactions = await _transactionCollection.Find(filter).ToListAsync();

            var result = transactions.Select(t => new TransactionHistoryDto
            {
                Id = t.Id.ToString(),
                Type = (TransactionType)t.Type,
                Status = (TransactionStatus)t.Status,
                Amount = t.Amount,
                TransactionCode = t.TransactionCode,
                DataTime = t.DataTime
            }).ToList();

            return result;
        }

        public async Task<List<TransactionHistoryRequestWithdrawOfUserDto>> GetAllRequestWithdrawAsync()
        {
            var filter = Builders<TransactionHistory>.Filter.Eq(t => t.Type, (int)TransactionType.Withdraw);
            var transactions = await _transactionCollection.Find(filter).ToListAsync();
            var walletIds = transactions.Select(x=>x.WalletId).Distinct().ToList();
            var users = await _userCollection.Find(x=>walletIds.Contains(x.WalletId)).ToListAsync();
            var userIds = users.Select(x=>x.Id).Distinct().ToList();
            var userBanks = await _userBankCollection.Find(x=>userIds.Contains(x.UserId)).ToListAsync();

            return transactions.Select(tr =>
            {
                var user = users.FirstOrDefault(x => x.WalletId.Equals(tr.WalletId));
                var userBank = userBanks.FirstOrDefault(x => x.UserId.Equals(user.Id));
                if (userBank == null) throw new Exception("UserBank is empty");
                return new TransactionHistoryRequestWithdrawOfUserDto
                {
                    WalletId = tr.WalletId,
                    Id = tr.Id,
                    Status = (TransactionStatus)tr.Status,
                    Type = (TransactionType)tr.Type,
                    DataTime = tr.DataTime,
                    Amount = tr.Amount,
                    UserId = user.Id,
                    UserName = user.Username,
                    BankId = userBank.BankId,
                    AccountBankName = userBank.AccountBankName,
                    BankNumber = userBank.BankNumber,
                    TransactionCode = tr.TransactionCode
                };
            }).ToList();
        }
    }
}
