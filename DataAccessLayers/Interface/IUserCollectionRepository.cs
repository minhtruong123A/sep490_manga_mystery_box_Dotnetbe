using BusinessObjects;
using BusinessObjects.Dtos.UserCollection;

namespace DataAccessLayers.Interface;

public interface IUserCollectionRepository : IGenericRepository<UserCollection>
{
    Task<List<UserCollectionGetAllDto>> GetAllWithDetailsAsync(string userId);
}