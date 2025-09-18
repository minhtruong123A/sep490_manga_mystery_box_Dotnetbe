using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class AuctionPaymentSessionRepository(MongoDbContext context)
    : GenericRepository<AuctionPaymentSession>(context.GetCollection<AuctionPaymentSession>("AuctionPaymentSession")),
        IAuctionPaymentSessionRepository
{
    private readonly IMongoCollection<AuctionPaymentSession> _auctionPaymentSessionCollection = context.GetCollection<AuctionPaymentSession>("AuctionPaymentSession");
}