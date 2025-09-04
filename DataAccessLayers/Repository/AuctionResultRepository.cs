using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class AuctionResultRepository(MongoDbContext context)
    : GenericRepository<AuctionResult>(context.GetCollection<AuctionResult>("AuctionResult")), IAuctionResultRepository
{
    private readonly IMongoCollection<AuctionResult> _auctionResultCollection = context.GetCollection<AuctionResult>("AuctionResult");
}