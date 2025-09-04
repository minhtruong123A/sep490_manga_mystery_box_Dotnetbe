using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Product;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class ProductService(
    IUnitOfWork unitOfWork,
    ISupabaseStorageHelper supabaseStorageHelper,
    IMapper mapper,
    IImageService imageService)
    : IProductService
{
    private readonly IMapper _mapper = mapper;
    private readonly ISupabaseStorageHelper _supabaseStorageHelper = supabaseStorageHelper;

    public async Task<ProductWithRarityDto?> GetProductWithRarityByIdAsync(string productId)
    {
        var dto = await unitOfWork.ProductRepository.GetProductWithRarityByIdAsync(productId);

        //if (!string.IsNullOrEmpty(dto.UrlImage))
        //{
        //    var signedUrl = await _supabaseStorageHelper.CreateSignedUrlAsync(dto.UrlImage);
        //    dto.UrlImage = signedUrl ?? dto.UrlImage;
        //}


        return dto;
    }

    public async Task<List<ProductWithRarityForModeratorDto>> GetAllProductsWithRarityAsync()
    {
        return await unitOfWork.ProductRepository.GetAllProductsWithRarityAsync();
    }

    public async Task<bool> CreateProductAsync(ProductCreateDto dto)
    {
        var rarityId = await unitOfWork.RarityRepository.GetRarityByNameAsync(dto.RarityName.ToLower());

        if (dto.UrlImage == null) throw new Exception("Image file is required.");

        var urlImage = await imageService.UploadModeratorProductOrMysteryBoxImageAsync(dto.UrlImage);

        var newProduct = new Product();
        newProduct.CollectionId = dto.CollectionId;
        newProduct.Description = dto.Description;
        newProduct.Name = dto.Name;
        newProduct.RarityId = rarityId;
        newProduct.UrlImage = urlImage;
        newProduct.CreatedAt = DateTime.Now;
        newProduct.UpdatedAt = DateTime.Now;
        newProduct.Is_Block = false;
        await unitOfWork.ProductRepository.AddAsync(newProduct);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> ChangeStatusProduct(string id)
    {
        var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
        if (product == null) return 0;


        product.Is_Block = !product.Is_Block;
        await unitOfWork.ProductRepository.UpdateAsync(product.Id, product);
        await unitOfWork.SaveChangesAsync();

        await unitOfWork.ProductRepository.CheckProduct(product.Id);
        return 1;
    }
}