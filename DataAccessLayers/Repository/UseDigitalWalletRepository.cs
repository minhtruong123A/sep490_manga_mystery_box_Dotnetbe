using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class UseDigitalWalletRepository : GenericRepository<UseDigitalWallet>, IUseDigitalWalletRepository
    {
        public UseDigitalWalletRepository(MongoDbContext context) : base(context.GetCollection<UseDigitalWallet>("UseDigitalWallet"))
        {
        }
    }
}
