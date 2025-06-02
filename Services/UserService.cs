using BusinessObjects;
using DataAccessLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _userRepository.GetAllAsync();
        }

        public Task<User> GetUserByIdAsync(string id)
        {
            return _userRepository.GetByIdAsync(id);
        }

        public Task CreateUserAsync(User user)
        {
            return _userRepository.CreateAsync(user);
        }

        public Task UpdateUserAsync(string id, User user)
        {
            return _userRepository.UpdateAsync(id, user);
        }

        public Task DeleteUserAsync(string id)
        {
            return _userRepository.DeleteAsync(id);
        }
    }
}
