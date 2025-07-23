using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.User;
using BusinessObjects.Enum;
using DataAccessLayers.Interface;
using DataAccessLayers.UnitOfWork;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Services.Helper.Supabase;
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
        private readonly IImageService _imageService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
        {
            _uniUnitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
        }

        //get all user
        public async Task<List<UserInformationDto>> GetAllUsersAsync()
        {
            var users = await _uniUnitOfWork.UserRepository.GetAllAsync();
            var userDtos = _mapper.Map<List<UserInformationDto>>(users);
            return userDtos;
        }

        //basic CRUD User
        //get profile by user ID
        public async Task<UserInformationDto> GetUserByIdAsync(string id)
        {
            var user = await _uniUnitOfWork.UserRepository.GetByIdAsync(id);
            var userDto = _mapper.Map<UserInformationDto>(user);
            return userDto;
        }

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

        public async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _uniUnitOfWork.UserRepository.GetByIdAsync(dto.UserId);
            if (user == null) throw new Exception("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurentPassword, user.Password))
            {
                return ChangePasswordResult.InvalidCurrentPassword;
            }

            if (!dto.NewPassword.Equals(dto.ConfirmPassword))
            {
                return ChangePasswordResult.PasswordMismatch;
            }
            dto.NewPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            return await _uniUnitOfWork.UserRepository.ChangePasswordAsync(dto);
        }

        public async Task<UserUpdateResponseDto> UpdateProfileAsync(IFormFile file, string userId, UserUpdateDto dto)
        {

            var user = await _uniUnitOfWork.UserRepository.FindOneAsync(x => x.Id == userId) ?? throw new Exception("User not found");
            var filePath = user.ProfileImage;
            if (file != null && file.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(user.ProfileImage)) await _imageService.DeleteProfileImageAsync(user.ProfileImage);

                filePath = await _imageService.UploadProfileImageAsync(file);
                user.ProfileImage = filePath;
            }
            var bank = await _uniUnitOfWork.UserBankRepository.FindOneAsync(x => x.UserId == userId);

            if (bank == null)
            {
                bank = new UserBank { UserId = userId };
                if (!string.IsNullOrWhiteSpace(dto.BankNumber)) bank.BankNumber = dto.BankNumber;
                if (!string.IsNullOrWhiteSpace(dto.AccountBankName)) bank.AccountBankName = dto.AccountBankName;
                if (!string.IsNullOrWhiteSpace(dto.BankId)) bank.BankId = dto.BankId;

                await _uniUnitOfWork.UserBankRepository.AddAsync(bank);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dto.BankNumber)) bank.BankNumber = dto.BankNumber;
                if (!string.IsNullOrWhiteSpace(dto.AccountBankName)) bank.AccountBankName = dto.AccountBankName;
                if (!string.IsNullOrWhiteSpace(dto.BankId)) bank.BankId = dto.BankId;

                await _uniUnitOfWork.UserBankRepository.UpdateAsync(bank.Id, bank);
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
/*            if (!string.IsNullOrWhiteSpace(dto.Username)) user.Username = dto.Username;*/

            await _uniUnitOfWork.UserRepository.UpdateAsync(userId, user);

            return new UserUpdateResponseDto()
            {
                Username = user.Username,
                ProfileImage = user.ProfileImage,
                PhoneNumber = user.PhoneNumber,
                AccountBankName = bank?.AccountBankName ?? "",
                BankNumber = bank?.BankNumber ?? "",
                Bankid = bank?.BankId ?? ""
            };
        }
    }
}
