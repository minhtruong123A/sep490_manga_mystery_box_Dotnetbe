using BusinessObjects.Dtos.Product;

namespace Services.Interface;

public interface IProductService
{
    Task<ProductWithRarityDto?> GetProductWithRarityByIdAsync(string productId);
    Task<List<ProductWithRarityForModeratorDto>> GetAllProductsWithRarityAsync();
    Task<bool> CreateProductAsync(ProductCreateDto dto);
    Task<int> changeStatusProduct(string id);
}