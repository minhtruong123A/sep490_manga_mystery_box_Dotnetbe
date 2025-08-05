using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IProductFavoriteRepository : IGenericRepository<ProductFavorite>
    {
        Task<List<UserCollectionGetAllDto>> GetFavoriteListWithDetailsAsync(string userId);
        Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId);
    }
}
