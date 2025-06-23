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

        public async Task<bool> CreateSellProductAsync(SellProductCreateDto dto, string userId) => await _uniUnitOfWork.SellProductRepository.CreateSellProductAsync(dto, userId);

        public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync()
        {
            var products = await _uniUnitOfWork.SellProductRepository.GetAllProductOnSaleAsync();
            await Task.WhenAll(products.Select(async p =>
            {
                if (!string.IsNullOrEmpty(p.UrlImage)) p.UrlImage = await _supabaseStorageHelper.CreateSignedUrlAsync(p.UrlImage);
            }));

            return products;
        }

        public async Task<SellProductDetailDto?> GetProductDetailByIdAsync(string id)
        {
            var product = await _uniUnitOfWork.SellProductRepository.GetProductDetailByIdAsync(id);
            //if (product is { UrlImage: not null and not "" }) cu phap moi .NET8+ moi biet @@
            if (product != null && !string.IsNullOrEmpty(product.UrlImage)) product.UrlImage = await _supabaseStorageHelper.CreateSignedUrlAsync(product.UrlImage);

            return product;
        }
    }
}
