using BusinessObjects;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context)
        {
            _users = context.Users;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }




        public async Task CreateAsync(User user)
        {
            if (string.IsNullOrEmpty(user.Id) || !ObjectId.TryParse(user.Id, out _))
            {
                user.Id = ObjectId.GenerateNewId().ToString();
            }

            await _users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(string id, User user)
        {
            await _users.ReplaceOneAsync(u => u.Id == id, user);
        }

        public async Task DeleteAsync(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
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
    }
}
