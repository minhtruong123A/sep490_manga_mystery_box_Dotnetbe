using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class CollectionRepository(MongoDbContext context)
    : GenericRepository<Collection>(context.GetCollection<Collection>("Collection")), ICollectionRepository;