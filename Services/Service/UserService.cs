using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.User;
using DataAccessLayers.Interface;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uniUnitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserInformationDto>> GetAllUsersAsync()
        {
            var users = await _uniUnitOfWork.UserRepository.GetAllAsync();
            var userDtos = _mapper.Map<List<UserInformationDto>>(users);
            return userDtos;
        }

        public Task<User?> GetUserByIdAsync(string id)
        {
            return _uniUnitOfWork.UserRepository.GetByIdAsync(id);
        }

        public Task CreateUserAsync(User user)
        {
            return _uniUnitOfWork.UserRepository.AddAsync(user);
        }

        public Task UpdateUserAsync(string id, User user)
        {
            return _uniUnitOfWork.UserRepository.UpdateAsync(id, user);
        }

        public Task DeleteUserAsync(string id)
        {
            return _uniUnitOfWork.UserRepository.DeleteAsync(id);
        }
    }
}
