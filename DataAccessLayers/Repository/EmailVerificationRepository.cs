using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class EmailVerificationRepository(MongoDbContext context)
    : GenericRepository<EmailVerification>(context.GetCollection<EmailVerification>("PendingEmailVerification")),
        IEmailVerificationRepository
{
    private readonly IMongoCollection<EmailVerification> _users = context.GetCollection<EmailVerification>("PendingEmailVerification");

    public async Task DeleteByEmailAsync(string email)
    {
        var result = await _collection.DeleteOneAsync(ev => ev.Email == email);
        if (result.DeletedCount == 0) throw new Exception("No email verification found with the given email.");
    }
}