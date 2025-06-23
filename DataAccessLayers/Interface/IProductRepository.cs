using BusinessObjects.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IProductRepository
    {
        Task<ProductWithRarityDto?> GetProductWithRarityByIdAsync(string productId);
    }
}
