using BusinessObjects.Dtos.UserBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserBoxService
    {
        Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId);
    }
}
