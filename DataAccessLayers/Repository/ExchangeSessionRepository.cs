using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class ExchangeSessionRepository(MongoDbContext context)
    : GenericRepository<ExchangeSession>(context.GetCollection<ExchangeSession>("ExchangeSession")),
        IExchangeSessionRepository;