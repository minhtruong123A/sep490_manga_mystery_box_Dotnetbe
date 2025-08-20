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
        private readonly IMongoClient _mongoClient;

        public PayOSRepository(MongoDbContext context, IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _walletCollection = context.GetCollection<UseDigitalWallet>("UseDigitalWallet");
            _transactionCollection = context.GetCollection<TransactionHistory>("TransactionHistory");
            _userCollection = context.GetCollection<User>("User");
        }

        public async Task<bool> RechargeWalletAsync2(string orderCode, int amount)
        {
            try
            {
                var transaction = await _transactionCollection.Find(x => x.TransactionCode == orderCode).FirstOrDefaultAsync();
                if (transaction == null) return false;

                var wallet = await _walletCollection.Find(x => x.Id == transaction.WalletId).FirstOrDefaultAsync();
                if (wallet == null) return false;

                if (transaction.Status == 2) return true;

                var update = Builders<UseDigitalWallet>.Update
                    .Inc(x => x.Ammount, amount)
                    .Set(x => x.IsActive, true);
                await _walletCollection.UpdateOneAsync(x => x.Id == wallet.Id, update);

                var updateTransaction = Builders<TransactionHistory>.Update.Set(x => x.Status, 2);
                await _transactionCollection.UpdateOneAsync(x => x.Id == transaction.Id, updateTransaction);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Recharge Error] {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RechargeWalletAsync(string orderCode, int amount)
        {
            using (var session = await _mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var transaction = await _transactionCollection
                        .Find(session, x => x.TransactionCode == orderCode)
                        .FirstOrDefaultAsync();

                    if (transaction == null)
                    {
                        await session.AbortTransactionAsync();
                        return false;
                    }

                    var wallet = await _walletCollection
                        .Find(session, x => x.Id == transaction.WalletId)
                        .FirstOrDefaultAsync();

                    if (wallet == null)
                    {
                        await session.AbortTransactionAsync();
                        return false;
                    }

                    if (transaction.Status == 2)
                    {
                        await session.AbortTransactionAsync();
                        return true;
                    }

                    var updateWallet = Builders<UseDigitalWallet>.Update
                        .Inc(x => x.Ammount, amount)
                        .Set(x => x.IsActive, true);
                    await _walletCollection.UpdateOneAsync(
                        session,
                        x => x.Id == wallet.Id,
                        updateWallet
                    );

                    var updateTransaction = Builders<TransactionHistory>.Update
                        .Set(x => x.Status, 2);
                    await _transactionCollection.UpdateOneAsync(
                        session,
                        x => x.Id == transaction.Id,
                        updateTransaction
                    );

                    await session.CommitTransactionAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await session.AbortTransactionAsync();
                    Console.WriteLine($"[Recharge Error] {ex.Message}");
                    return false;
                }
            }             
        }
    }
}
