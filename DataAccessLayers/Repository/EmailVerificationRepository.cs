using BusinessObjects;
using DataAccessLayers.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class EmailVerificationRepository : GenericRepository<EmailVerification>, IEmailVerificationRepository
    {
        private readonly IMongoCollection<EmailVerification> _users;

        public EmailVerificationRepository(MongoDbContext context) : base(context.GetCollection<EmailVerification>("PendingEmailVerification"))
        {
            _users = context.GetCollection<EmailVerification>("PendingEmailVerification");
        }

        public async Task DeleteByEmailAsync(string email)
        {
            var result = await _collection.DeleteOneAsync(ev => ev.Email == email);
            if (result.DeletedCount == 0)
            {
                throw new Exception("No email verification found with the given email.");
            }
        }
    }
}
