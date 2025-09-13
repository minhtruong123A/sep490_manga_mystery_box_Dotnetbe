using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserBox;

namespace Services.Interface;

public interface IUserBoxService
{
    Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId);
    Task<ProductResultDto> OpenMysteryBoxAsync(string userBoxId, string userId);
}