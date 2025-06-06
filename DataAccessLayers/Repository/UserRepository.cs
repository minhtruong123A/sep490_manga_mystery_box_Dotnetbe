using BusinessObjects;
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

        //Authen
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
    }
}
