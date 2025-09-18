using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserBox;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class UserBoxService(IUnitOfWork uniUnitOfWork) : IUserBoxService
{
    public async Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId)
    {
        return await uniUnitOfWork.UserBoxRepository.GetAllWithDetailsAsync(userId);
    }

    public async Task<ProductResultDto> OpenMysteryBoxAsync(string userBoxId, string userId)
    {
        return await uniUnitOfWork.UserBoxRepository.OpenMysteryBoxAsync(userBoxId, userId);
    }
}