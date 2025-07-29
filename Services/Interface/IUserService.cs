using BusinessObjects;
using BusinessObjects.Dtos.User;
using BusinessObjects.Enum;
using Microsoft.AspNetCore.Http;
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
        Task<UserInformationDto> GetUserByIdAsync(string id);
        Task<UserInformationDto> GetOtherUserByIdAsync(string id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(string id, User user);
        Task DeleteUserAsync(string id);
        Task DeleteUserByEmailAsync(string email);
        Task<ChangePasswordResult> ChangePasswordAsync(string userId,ChangePasswordDto dto);
        Task<UserUpdateResponseDto> UpdateProfileAsync(IFormFile file, string userId, UserUpdateDto dto);
    }
}
