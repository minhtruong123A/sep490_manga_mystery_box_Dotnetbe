using BusinessObjects;
using BusinessObjects.Dtos.Product;
using DataAccessLayers.Interface;
using Services.Helper.Supabase;
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
                private readonly ISupabaseStorageHelper _supabaseStorageHelper;
                public SellProductService(IUnitOfWork unitOfWork, ISupabaseStorageHelper storageHelper)
                {
                        _uniUnitOfWork = unitOfWork;
                        _supabaseStorageHelper = storageHelper;
                }

                public async Task<int> CreateSellProductAsync(SellProductCreateDto dto, string userId) => await _uniUnitOfWork.SellProductRepository.CreateSellProductAsync(dto, userId);

                public async Task<bool> UpdateSellProductAsync(UpdateSellProductDto dto) => await _uniUnitOfWork.SellProductRepository.UpdateSellProductAsync(dto);

                public async Task<bool> ChangestatusSellProductAsync(string id) => await _uniUnitOfWork.SellProductRepository.ChangestatusSellProductAsync(id);

                public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleOfUserAsync(string id) => await _uniUnitOfWork.SellProductRepository.GetAllProductOnSaleOfUserIdAsync(id);

                public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync() => await _uniUnitOfWork.SellProductRepository.GetAllProductOnSaleAsync();

                public async Task<SellProductDetailDto?> GetProductDetailByIdAsync(string id) => await _uniUnitOfWork.SellProductRepository.GetProductDetailByIdAsync(id);
        ////if (product is { UrlImage: not null and not "" }) cu phap moi .NET8+ moi biet @@
        //if (product != null && !string.IsNullOrEmpty(product.UrlImage)) product.UrlImage = await _supabaseStorageHelper.CreateSignedUrlAsync(product.UrlImage);

                public async Task<string> BuySellProductAsync(string buyerId, string sellProductId, int quantity) => await _uniUnitOfWork.SellProductRepository.BuySellProductAsync(buyerId, sellProductId, quantity);


                public async Task<bool> CancelSellProductAsync(string sellProductId) => await _uniUnitOfWork.SellProductRepository.CancelSellProductAsync(sellProductId);

    }
}
