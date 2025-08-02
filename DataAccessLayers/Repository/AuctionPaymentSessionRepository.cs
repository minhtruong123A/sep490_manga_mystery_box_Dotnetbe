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
    public class AuctionPaymentSessionRepository : GenericRepository<AuctionPaymentSession>, IAuctionPaymentSessionRepository
    {
        private readonly IMongoCollection<AuctionPaymentSession> _auctionPaymentSessionCollection;
        public AuctionPaymentSessionRepository(MongoDbContext context) : base(context.GetCollection<AuctionPaymentSession>("AuctionPaymentSession"))
        {
            _auctionPaymentSessionCollection = context.GetCollection<AuctionPaymentSession>("AuctionPaymentSession");
        }
    }
}
