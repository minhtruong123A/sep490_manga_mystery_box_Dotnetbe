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

        //get all user
        public async Task<List<UserInformationDto>> GetAllUsersAsync()
        {
            var users = await _uniUnitOfWork.UserRepository.GetAllAsync();
            var userDtos = _mapper.Map<List<UserInformationDto>>(users);
            return userDtos;
        }

        //basic CRUD User
        public async Task<User?> GetUserByIdAsync(string id) =>  await _uniUnitOfWork.UserRepository.GetByIdAsync(id);
        

        public async Task CreateUserAsync(User user) => await _uniUnitOfWork.UserRepository.AddAsync(user);


        public async Task UpdateUserAsync(string id, User user) => await _uniUnitOfWork.UserRepository.UpdateAsync(id, user);


        public async Task DeleteUserAsync(string id) => await _uniUnitOfWork.UserRepository.DeleteAsync(id);


        //delete user and email verification by email
        public async Task DeleteUserByEmailAsync(string email)
        {
            _ = await _uniUnitOfWork.UserRepository.GetByEmailAsync(email) ?? throw new Exception("User not found");
            await _uniUnitOfWork.UserRepository.DeleteByEmailAsync(email);
            await _uniUnitOfWork.EmailVerificationRepository.DeleteByEmailAsync(email);
        }
    }
}
