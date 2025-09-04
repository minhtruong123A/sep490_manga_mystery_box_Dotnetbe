using AutoMapper;
using BusinessObjects.Dtos.Product;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class UserProductService(IUnitOfWork unitOfWork, IMapper mapper) : IUserProductService
{
    private readonly IMapper _mapper = mapper;

    public async Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string id, string collectionId)
    {
        return await unitOfWork.UserProductRepository.GetAllWithDetailsAsync(id, collectionId);
    }

    public async Task<bool> CheckedUpdateQuantityAsync(string userProductId)
    {
        var userProduct = await unitOfWork.UserProductRepository.GetByIdAsync(userProductId);
        if (userProduct == null) throw new Exception("User product not found!");

        userProduct.isQuantityUpdateInc = false;
        await unitOfWork.UserProductRepository.UpdateAsync(userProductId, userProduct);
        await unitOfWork.SaveChangesAsync();

        return true;
    }
}