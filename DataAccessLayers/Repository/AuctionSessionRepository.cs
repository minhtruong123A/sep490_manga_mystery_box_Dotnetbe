using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class AuctionSessionRepository(MongoDbContext context)
    : GenericRepository<AuctionSession>(context.GetCollection<AuctionSession>("AuctionSession")),
        IAuctionSessionRepository;