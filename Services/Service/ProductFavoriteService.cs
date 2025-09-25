using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class ProductFavoriteService(IUnitOfWork unitOfWork) : IProductFavoriteService
{
    public async Task<List<UserCollectionGetAllDto>> GetFavoriteListWithDetailsAsync(string userId)
    {
        return await unitOfWork.ProductFavoriteRepository.GetFavoriteListWithDetailsAsync(userId);
    }

    public async Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId)
    {
        return await unitOfWork.ProductFavoriteRepository.GetAllWithDetailsAsync(userId);
    }

    public async Task<bool> CreateAsync(string userId, string userProductId)
    {
        var favorites = await unitOfWork.ProductFavoriteRepository.GetAllAsync();
        var existFavorite = favorites
            .FirstOrDefault(x => x.User_Id.Equals(userId) && x.User_productId.Equals(userProductId));
        var existUserProduct = await unitOfWork.UserProductRepository.GetByIdAsync(userProductId);
        if (existFavorite != null) return false;
        if (existUserProduct != null) return false;
        var newFavorite = new ProductFavorite { User_Id = userId, User_productId = userProductId };
        await unitOfWork.ProductFavoriteRepository.AddAsync(newFavorite);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string favoriteId)
    {
        await unitOfWork.ProductFavoriteRepository.DeleteAsync(favoriteId);
        return true;
    }
}