using BusinessObjects;
using BusinessObjects.Dtos.UserCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IUserCollectionRepository : IGenericRepository<UserCollection>
    {
        Task<List<UserCollectionGetAllDto>> GetAllWithDetailsAsync(string userId);
    }
}
