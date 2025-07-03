using BusinessObjects;
using BusinessObjects.Dtos.Product;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IUserProductRepository : IGenericRepository<UserProduct>
    {
        Task AddManyAsync(IEnumerable<UserProduct> items);
        Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId, string collectionId);
        Task<UserProduct> FindOneAsync(Expression<Func<UserProduct, bool>> filter);
        Task UpdateOneAsync(string id, UpdateDefinition<UserProduct> update);
        Task AddAsync(UserProduct userProduct);
    }
}
