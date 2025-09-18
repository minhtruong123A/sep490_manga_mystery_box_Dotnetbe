using BusinessObjects;
using BusinessObjects.Dtos.Collection;
using BusinessObjects.Dtos.UserCollection;

namespace Services.Interface;

public interface IUserCollectionService
{
    Task CreateUserCollectionAsync(UserCollection collection);
    Task<List<UserCollectionGetAllDto>> GetAllWithDetailsAsync(string id);
    Task CreateUserCollectionByUserAsync(string userId, CollectionCreateByUserDto dto);
}