using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class ProductInMangaBoxRepository(MongoDbContext context)
    : GenericRepository<ProductInMangaBox>(context.GetCollection<ProductInMangaBox>("ProductInMangaBox")),
        IProductInMangaBoxRepository;