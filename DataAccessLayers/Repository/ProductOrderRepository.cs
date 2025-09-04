using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class ProductOrderRepository(MongoDbContext context)
    : GenericRepository<ProductOrder>(context.GetCollection<ProductOrder>("ProductOrder")), IProductOrderRepository;