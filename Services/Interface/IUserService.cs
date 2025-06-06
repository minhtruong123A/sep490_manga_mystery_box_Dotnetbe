using BusinessObjects;
using BusinessObjects.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserService
    {
        Task<List<UserInformationDto>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(string id, User user);
        Task DeleteUserAsync(string id);
        Task DeleteUserByEmailAsync(string email);
    }
}
