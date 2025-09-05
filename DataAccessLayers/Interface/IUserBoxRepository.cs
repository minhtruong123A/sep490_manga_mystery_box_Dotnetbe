using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserBox;

namespace DataAccessLayers.Interface;

public interface IUserBoxRepository : IGenericRepository<UserBox>
{
    Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId);
    Task<ProductResultDto> OpenMysteryBoxAsync(string userBoxId, string userId);
}