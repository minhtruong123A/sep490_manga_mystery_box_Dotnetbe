using BusinessObjects;
using BusinessObjects.Dtos.UserBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IUserBoxRepository : IGenericRepository<UserBox>
    {
        Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId);
    }
}
