using BusinessObjects;
using BusinessObjects.Dtos.User;
using BusinessObjects.Enum;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context) : base(context.GetCollection<User>("User"))
        {
            _users = context.GetCollection<User>("User");
        }

        public async Task<User?> GetSystemAccountByAccountEmailAndPassword(string accountEmail, string password)
        {
            return await _users.Find(m => m.Email == accountEmail && m.Password == password).SingleOrDefaultAsync();
        }

        public async Task<User?> GetSystemAccountByAccountName(string accountName)
        {
            return await _users.Find(m => m.Username == accountName).SingleOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task DeleteByEmailAsync(string email)
        {
            var result = await _users.DeleteOneAsync(u => u.Email == email);
            if (result.DeletedCount == 0)
            {
                throw new Exception("No User found with the given email.");
            }
        }

        public async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _users.Find(x => x.Id.Equals(dto.UserId)).FirstOrDefaultAsync();
            if (user == null) throw new Exception("User not found");

            if (user.Password.Equals(dto.CurentPassword))
            {
                return ChangePasswordResult.InvalidCurrentPassword;
            }

            if (!dto.NewPassword.Equals(dto.ConfirmPassword))
            {
                return ChangePasswordResult.PasswordMismatch;
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, dto.UserId);
            var update = await _users.UpdateOneAsync(filter, Builders<User>.Update.Set(x => x.Password, dto.NewPassword));

            if (update.ModifiedCount == 0)
            {
                throw new Exception($"Failed to change password for user: {user.Username}");
            }

            return ChangePasswordResult.Success;
        }

    }
}
