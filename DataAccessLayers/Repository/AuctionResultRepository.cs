using BusinessObjects;
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
    public class AuctionResultRepository : GenericRepository<AuctionResult>, IAuctionResultRepository
    {
        private readonly IMongoCollection<AuctionResult> _auctionResultCollection;
        public AuctionResultRepository(MongoDbContext context) : base(context.GetCollection<AuctionResult>("AuctionResult"))
        {
            _auctionResultCollection = context.GetCollection<AuctionResult>("AuctionResult");
        }
    }
}
