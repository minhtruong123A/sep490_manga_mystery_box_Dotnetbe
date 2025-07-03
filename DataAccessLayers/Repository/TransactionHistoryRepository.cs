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
        public TransactionHistoryRepository(MongoDbContext context) : base(context.GetCollection<TransactionHistory>("TransactionHistory"))
        {
            _transactionCollection = context.GetCollection<TransactionHistory>("TransactionHistory");
        }

        public async Task<List<TransactionHistoryDto>> GetTransactionsByWalletIdAsync(string walletId)
        {
            var filter = Builders<TransactionHistory>.Filter.And(
                  Builders<TransactionHistory>.Filter.Eq(t => t.WalletId, walletId),
                  Builders<TransactionHistory>.Filter.Eq(t => t.Type, (int)TransactionType.Recharge)
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
    }
}
