using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;

namespace DataAccessLayers.Interface;

public interface IProductFavoriteRepository : IGenericRepository<ProductFavorite>
{
    Task<List<UserCollectionGetAllDto>> GetFavoriteListWithDetailsAsync(string userId);
    Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId);
}