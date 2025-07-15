using BusinessObjects.Dtos.Product;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using Services.Helper.Supabase;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupabaseStorageHelper _supabaseStorageHelper;

        public ProductService(IUnitOfWork unitOfWork, ISupabaseStorageHelper supabaseStorageHelper)
        {
            _unitOfWork = unitOfWork;
            _supabaseStorageHelper = supabaseStorageHelper;
        }

        public async Task<ProductWithRarityDto?> GetProductWithRarityByIdAsync(string productId)
        {
            var dto = await _unitOfWork.ProductRepository.GetProductWithRarityByIdAsync(productId);
            if (dto == null) return null;

            //if (!string.IsNullOrEmpty(dto.UrlImage))
            //{
            //    var signedUrl = await _supabaseStorageHelper.CreateSignedUrlAsync(dto.UrlImage);
            //    dto.UrlImage = signedUrl ?? dto.UrlImage;
            //}


            return dto;
        }
    }
}
