using BusinessObjects;
using BusinessObjects.Dtos.User;
using BusinessObjects.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetSystemAccountByAccountEmailAndPassword(string accountEmail, string password);
        Task<User?> GetSystemAccountByAccountName(string accountName);
        Task<User?> GetByEmailAsync(string email);
        Task DeleteByEmailAsync(string email);
        Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordDto dto);
    }
}
