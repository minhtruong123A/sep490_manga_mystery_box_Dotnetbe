using BusinessObjects.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProductService
    {
        Task<ProductWithRarityDto?> GetProductWithRarityByIdAsync(string productId);
        Task<List<ProductWithRarityForModeratorDto>> GetAllProductsWithRarityAsync();
        Task<int> changeStatusProduct(string id);
    }
}
