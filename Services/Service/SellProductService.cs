using BusinessObjects.Dtos.Product;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class SellProductService(IUnitOfWork unitOfWork)
    : ISellProductService
{
    public async Task<int> CreateSellProductAsync(SellProductCreateDto dto, string userId)
    {
        return await unitOfWork.SellProductRepository.CreateSellProductAsync(dto, userId);
    }

    public async Task<bool> UpdateSellProductAsync(UpdateSellProductDto dto)
    {
        return await unitOfWork.SellProductRepository.UpdateSellProductAsync(dto);
    }

    public async Task<bool> ChangeStatusSellProductAsync(string id)
    {
        return await unitOfWork.SellProductRepository.ChangestatusSellProductAsync(id);
    }

    public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleOfUserAsync(string id)
    {
        return await unitOfWork.SellProductRepository.GetAllProductOnSaleOfUserIdAsync(id);
    }

    public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync()
    {
        return await unitOfWork.SellProductRepository.GetAllProductOnSaleAsync();
    }

    public async Task<SellProductDetailDto?> GetProductDetailByIdAsync(string id)
    {
        return await unitOfWork.SellProductRepository.GetProductDetailByIdAsync(id);
    }
    ////if (product is { UrlImage: not null and not "" }) cu phap moi .NET8+ moi biet @@
    //if (product != null && !string.IsNullOrEmpty(product.UrlImage)) product.UrlImage = await _supabaseStorageHelper.CreateSignedUrlAsync(product.UrlImage);

    public async Task<string> BuySellProductAsync(string buyerId, string sellProductId, int quantity)
    {
        return await unitOfWork.SellProductRepository.BuySellProductAsync(buyerId, sellProductId, quantity);
    }

    public async Task<List<SellProductGetAllDto>> GetAllSellProductSuggestionsAsync(string userId)
    {
        return await unitOfWork.SellProductRepository.GetAllSellProductSuggestionsAsync(userId);
    }


    public async Task<bool> CancelSellProductAsync(string sellProductId)
    {
        return await unitOfWork.SellProductRepository.CancelSellProductAsync(sellProductId);
    }
}