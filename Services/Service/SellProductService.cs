using BusinessObjects;
using BusinessObjects.Dtos.Product;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class SellProductService : ISellProductService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        public SellProductService(IUnitOfWork unitOfWork)
        {
            _uniUnitOfWork = unitOfWork;
        }

        public async Task<bool> CreateSellProductAsync(SellProductCreateDto dto, string userId) => await _uniUnitOfWork.SellProductRepository.CreateSellProductAsync(dto, userId);
        public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync() => await _uniUnitOfWork.SellProductRepository.GetAllProductOnSaleAsync();
        public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleOfUserAsync(string id) => await _uniUnitOfWork.SellProductRepository.GetAllProductOnSaleOfUserIdAsync(id);
        public async Task<SellProductDetailDto> GetProductDetailByIdAsync(string id) => await _uniUnitOfWork.SellProductRepository.GetProductDetailByIdAsync(id);

    }
}
