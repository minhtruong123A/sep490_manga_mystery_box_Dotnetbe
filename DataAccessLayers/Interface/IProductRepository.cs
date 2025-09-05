using BusinessObjects;
using BusinessObjects.Dtos.Product;

namespace DataAccessLayers.Interface;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<ProductWithRarityDto?> GetProductWithRarityByIdAsync(string productId);
    Task<List<ProductWithRarityForModeratorDto>> GetAllProductsWithRarityAsync();
    Task CheckProduct(string productId);
}