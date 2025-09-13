using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.User;
using BusinessObjects.Enum;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Http;
using Services.Interface;

namespace Services.Service;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
    : IUserService
{
    //get all user
    public async Task<List<UserInformationDto>> GetAllUsersAsync()
    {
        var users = await unitOfWork.UserRepository.GetAllAsync();
        var userDtos = mapper.Map<List<UserInformationDto>>(users);
        return userDtos;
    }

    //basic CRUD User
    //get profile by user ID
    public async Task<UserInformationDto> GetUserByIdAsync(string id)
    {
        var userBanks = await unitOfWork.UserBankRepository.GetAllAsync();
        var accountBank = userBanks.Where(x => x.UserId.Equals(id)).FirstOrDefault();

        var user = await unitOfWork.UserRepository.GetByIdAsync(id);
        var userDto = mapper.Map<UserInformationDto>(user);
        if (accountBank != null)
        {
            userDto.BankId = accountBank.BankId;
            userDto.Banknumber = accountBank.BankNumber;
            userDto.AccountBankName = accountBank.AccountBankName;
        }

        return userDto;
    }

    public async Task<UserInformationDto> GetOtherUserByIdAsync(string id)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(id);
        var userDto = mapper.Map<UserInformationDto>(user);
        return userDto;
    }

    public async Task CreateUserAsync(User user)
    {
        await unitOfWork.UserRepository.AddAsync(user);
    }


    public async Task UpdateUserAsync(string id, User user)
    {
        await unitOfWork.UserRepository.UpdateAsync(id, user);
    }


    public async Task DeleteUserAsync(string id)
    {
        await unitOfWork.UserRepository.DeleteAsync(id);
    }


    //delete user and email verification by email
    public async Task DeleteUserByEmailAsync(string email)
    {
        _ = await unitOfWork.UserRepository.GetByEmailAsync(email) ?? throw new Exception("User not found");
        await unitOfWork.UserRepository.DeleteByEmailAsync(email);
        await unitOfWork.EmailVerificationRepository.DeleteByEmailAsync(email);
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurentPassword, user.Password))
            return ChangePasswordResult.InvalidCurrentPassword;

        if (!dto.NewPassword.Equals(dto.ConfirmPassword)) return ChangePasswordResult.PasswordMismatch;
        dto.NewPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        return await unitOfWork.UserRepository.ChangePasswordAsync(userId, dto);
    }

    public async Task<UserUpdateResponseDto> UpdateProfileAsync(IFormFile file, string userId, UserUpdateDto dto)
    {
        var user = await unitOfWork.UserRepository.FindOneAsync(x => x.Id == userId) ??
                   throw new Exception("User not found");
        var filePath = user.ProfileImage;
        if (file != null && file.Length > 0)
        {
            if (!string.IsNullOrWhiteSpace(user.ProfileImage))
                await imageService.DeleteProfileImageAsync(user.ProfileImage);

            filePath = await imageService.UploadProfileImageAsync(file);
            user.ProfileImage = filePath;
        }

        var bank = await unitOfWork.UserBankRepository.FindOneAsync(x => x.UserId == userId);

        if (bank == null)
        {
            bank = new UserBank { UserId = userId };
            if (!string.IsNullOrWhiteSpace(dto.BankNumber)) bank.BankNumber = dto.BankNumber;
            if (!string.IsNullOrWhiteSpace(dto.AccountBankName)) bank.AccountBankName = dto.AccountBankName;
            if (!string.IsNullOrWhiteSpace(dto.BankId)) bank.BankId = dto.BankId;

            await unitOfWork.UserBankRepository.AddAsync(bank);
            await unitOfWork.SaveChangesAsync();
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(dto.BankNumber)) bank.BankNumber = dto.BankNumber;
            if (!string.IsNullOrWhiteSpace(dto.AccountBankName)) bank.AccountBankName = dto.AccountBankName;
            if (!string.IsNullOrWhiteSpace(dto.BankId)) bank.BankId = dto.BankId;

            await unitOfWork.UserBankRepository.UpdateAsync(bank.Id, bank);
            await unitOfWork.SaveChangesAsync();
        }

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
/*            if (!string.IsNullOrWhiteSpace(dto.Username)) user.Username = dto.Username;*/

        await unitOfWork.UserRepository.UpdateAsync(userId, user);
        await unitOfWork.SaveChangesAsync();

        return new UserUpdateResponseDto
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