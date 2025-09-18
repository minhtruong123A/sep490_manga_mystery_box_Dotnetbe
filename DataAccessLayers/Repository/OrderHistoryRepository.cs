using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class OrderHistoryRepository(MongoDbContext context)
    : GenericRepository<OrderHistory>(context.GetCollection<OrderHistory>("OrderHistory")), IOrderHistoryRepository;