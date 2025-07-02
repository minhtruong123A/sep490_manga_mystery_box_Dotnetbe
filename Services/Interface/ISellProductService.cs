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
        Task<int> CreateSellProductAsync(SellProductCreateDto dto, string userId);
        Task<bool> UpdateSellProductAsync(UpdateSellProductDto dto);
        Task<bool> ChangestatusSellProductAsync(string id);
        Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync();
        Task<List<SellProductGetAllDto>> GetAllProductOnSaleOfUserAsync(string id);
        Task<SellProductDetailDto> GetProductDetailByIdAsync(string id);

    }
}
