using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class UseDigitalWalletRepository : GenericRepository<UseDigitalWallet>, IUseDigitalWalletRepository
    {
        private readonly IMongoCollection<UseDigitalWallet> _walletCollection;

        public UseDigitalWalletRepository(MongoDbContext context) : base(context.GetCollection<UseDigitalWallet>("UseDigitalWallet"))
        {
            _walletCollection = context.GetCollection<UseDigitalWallet>("UseDigitalWallets");
        }

        public async Task<UseDigitalWallet?> GetWalletByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return null;

            return await _walletCollection.AsQueryable().FirstOrDefaultAsync(w => w.Id == objectId.ToString());
        }
    }
}
