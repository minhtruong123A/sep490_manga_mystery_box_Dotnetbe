using BusinessObjects.Mongodb;
using BusinessObjects;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository
{
    public class PayOSRepository : IPayOSRepository
    {
        private readonly IMongoCollection<UseDigitalWallet> _walletCollection;
        private readonly IMongoCollection<TransactionHistory> _transactionCollection;
        private readonly IMongoCollection<User> _userCollection;

        public PayOSRepository(MongoDbContext context)
        {
            _walletCollection = context.GetCollection<UseDigitalWallet>("UseDigitalWallet");
            _transactionCollection = context.GetCollection<TransactionHistory>("TransactionHistory");
            _userCollection = context.GetCollection<User>("User");
        }

        public async Task<bool> RechargeWalletAsync(string orderCode, int amount)
        {
            var transaction = await _transactionCollection
                .Find(x => x.TransactionCode == orderCode)
                .FirstOrDefaultAsync();
            if (transaction == null) return false;
            var wallet = await _walletCollection
                .Find(x => x.Id == transaction.WalletId)
                .FirstOrDefaultAsync();
            if (wallet == null) return false;
            if (transaction.Status == 2) return true;

            var update = Builders<UseDigitalWallet>.Update.Inc(x => x.Ammount, amount).Set(x => x.IsActive, true);
            await _walletCollection.UpdateOneAsync(x => x.Id == wallet.Id, update);
            var updateTransaction = Builders<TransactionHistory>.Update.Set(x => x.Status, 2);
            await _transactionCollection.UpdateOneAsync(x => x.Id == transaction.Id, updateTransaction);

            return true;
        }
    }
}
