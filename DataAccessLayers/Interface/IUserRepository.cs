using BusinessObjects;
using BusinessObjects.Dtos.User;
using BusinessObjects.Enum;

namespace DataAccessLayers.Interface;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetSystemAccountByAccountEmailAndPassword(string accountEmail, string password);
    Task<User?> GetSystemAccountByAccountName(string accountName);
    Task<User?> GetByEmailAsync(string email);
    Task DeleteByEmailAsync(string email);
    Task<ChangePasswordResult> ChangePasswordAsync(string userId, ChangePasswordDto dto);
}