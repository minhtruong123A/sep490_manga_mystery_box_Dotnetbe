using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class BoxOrderRepository(MongoDbContext context)
    : GenericRepository<BoxOrder>(context.GetCollection<BoxOrder>("BoxOrder")), IBoxOrderRepository;