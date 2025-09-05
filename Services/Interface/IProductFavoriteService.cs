using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;

namespace Services.Interface;

public interface IProductFavoriteService
{
    Task<List<UserCollectionGetAllDto>> GetFavoriteListWithDetailsAsync(string userId);
    Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId);
    Task<bool> CreateAsync(string userId, string userProductId);
    Task<bool> DeleteAsync(string favoriteId);
}