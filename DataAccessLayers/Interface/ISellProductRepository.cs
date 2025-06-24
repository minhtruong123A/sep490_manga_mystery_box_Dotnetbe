using BusinessObjects;
using BusinessObjects.Dtos.Product;
using DataAccessLayers.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface ISellProductRepository : IGenericRepository<SellProduct>
    {
        Task<int> CreateSellProductAsync(SellProductCreateDto dto, string userId);
        Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync();
        Task<SellProductDetailDto?> GetProductDetailByIdAsync(string id);
    }
}
