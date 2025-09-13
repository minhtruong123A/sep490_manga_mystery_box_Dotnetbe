using BusinessObjects;
using BusinessObjects.Dtos.User;
using BusinessObjects.Enum;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class UserRepository(MongoDbContext context)
    : GenericRepository<User>(context.GetCollection<User>("User")), IUserRepository
{
    private readonly IMongoCollection<User> _users = context.GetCollection<User>("User");

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
        if (result.DeletedCount == 0) throw new Exception("No User found with the given email.");
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
        var update = await _users.UpdateOneAsync(filter, Builders<User>.Update.Set(x => x.Password, dto.NewPassword));

        return update.ModifiedCount == 0 ? throw new Exception("Failed to change password for user;") : ChangePasswordResult.Success;
    }
}