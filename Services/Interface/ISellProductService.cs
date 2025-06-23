using BusinessObjects;
using BusinessObjects.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISellProductService
    {
        Task<bool> CreateSellProductAsync(SellProductCreateDto dto, string userId);
        Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync();
        Task<List<SellProductGetAllDto>> GetAllProductOnSaleOfUserAsync(string id);
        Task<SellProductDetailDto> GetProductDetailByIdAsync(string id);

    }
}
