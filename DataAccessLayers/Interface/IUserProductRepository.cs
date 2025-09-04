using System.Linq.Expressions;
using BusinessObjects;
using BusinessObjects.Dtos.Product;
using MongoDB.Driver;

namespace DataAccessLayers.Interface;

public interface IUserProductRepository : IGenericRepository<UserProduct>
{
    Task AddManyAsync(IEnumerable<UserProduct> items);
    Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId, string collectionId);
    Task<UserProduct> FindOneAsync(Expression<Func<UserProduct, bool>> filter);
    Task UpdateOneAsync(string id, UpdateDefinition<UserProduct> update);
    Task AddAsync(UserProduct userProduct);
}