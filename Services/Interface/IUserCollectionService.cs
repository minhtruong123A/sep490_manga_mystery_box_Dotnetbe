using BusinessObjects;
using BusinessObjects.Dtos.Collection;
using BusinessObjects.Dtos.UserCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserCollectionService
    {
        Task CreateUserCollectionAsync(UserCollection collection);
        Task<List<UserCollectionGetAllDto>> GetAllWithDetailsAsync(string id);
        Task CreateUserCollectionByUserAsync(string userId, CollectionCreateByUserDto dto);
    }
}
