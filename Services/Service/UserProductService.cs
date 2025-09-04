using AutoMapper;
using BusinessObjects.Dtos.Product;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class UserProductService : IUserProductService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uniUnitOfWork;

    public UserProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _uniUnitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string id, string collectionId)
    {
        return await _uniUnitOfWork.UserProductRepository.GetAllWithDetailsAsync(id, collectionId);
    }

    public async Task<bool> CheckedUpdateQuantityAsync(string userProductId)
    {
        var userProduct = await _uniUnitOfWork.UserProductRepository.GetByIdAsync(userProductId);
        if (userProduct == null) throw new Exception("User product not found!");

        userProduct.isQuantityUpdateInc = false;
        await _uniUnitOfWork.UserProductRepository.UpdateAsync(userProductId, userProduct);
        await _uniUnitOfWork.SaveChangesAsync();

        return true;
    }
}