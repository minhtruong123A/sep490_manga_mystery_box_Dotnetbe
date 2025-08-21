using BusinessObjects;
using AutoMapper;
using BusinessObjects.Dtos.Product;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using Services.AutoMapper;
using Services.Helper.Supabase;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupabaseStorageHelper _supabaseStorageHelper;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, ISupabaseStorageHelper supabaseStorageHelper, IMapper mapper, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _supabaseStorageHelper = supabaseStorageHelper;
            _mapper = mapper;
            _imageService = imageService;
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
        public async Task<List<ProductWithRarityForModeratorDto>> GetAllProductsWithRarityAsync() => await _unitOfWork.ProductRepository.GetAllProductsWithRarityAsync();
        public async Task<bool> CreateProductAsync(ProductCreateDto dto)
        {
            var rarityId = await _unitOfWork.RarityRepository.GetRarityByNameAsync(dto.RarityName.ToLower());

            if (dto.UrlImage == null)
            {
                throw new Exception("Image file is required.");
            }

            var urlImage = await _imageService.UploadModeratorProductOrMysteryBoxImageAsync(dto.UrlImage);

            var newProduct = new Product();
            newProduct.CollectionId = dto.CollectionId;
            newProduct.Description = dto.Description;
            newProduct.Name = dto.Name;
            newProduct.RarityId = rarityId;
            newProduct.UrlImage = urlImage;
            newProduct.CreatedAt = DateTime.Now;
            newProduct.UpdatedAt = DateTime.Now;
            newProduct.Is_Block=false;
            await _unitOfWork.ProductRepository.AddAsync(newProduct);
            await _unitOfWork.SaveChangesAsync();
            return true;

        }
        public async Task<int> changeStatusProduct(string id)
        {
            var product =  await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null) return 0;


            if (product.Is_Block) 
            {
                product.Is_Block = false;
            }else
            {
                product.Is_Block = true;
            }
            await _unitOfWork.ProductRepository.UpdateAsync(product.Id,product);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.ProductRepository.CheckProduct(product.Id);
            return 1;
        }
    }
}
