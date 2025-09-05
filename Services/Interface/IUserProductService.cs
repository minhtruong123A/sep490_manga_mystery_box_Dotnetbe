using BusinessObjects.Dtos.Product;

namespace Services.Interface;

public interface IUserProductService
{
    Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string id, string collectionId);
    Task<bool> CheckedUpdateQuantityAsync(string userProductId);
}