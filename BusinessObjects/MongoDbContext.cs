using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Configuration;

namespace BusinessObjects
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        public MongoDbContext(IConfiguration configuration)
        {
            try
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                Console.WriteLine($"Connecting to MongoDb with connection string:\n{connectionString}");

                _client = new MongoClient(connectionString);

                var databaseName = configuration["MongoDbName"];
                _database = _client.GetDatabase(databaseName);

                Console.WriteLine("MongoDb success");
            }
            catch (Exception ex)
            {
                Console.WriteLine("MongoDb connection failed:");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public IMongoCollection<EmailVerification> EmailVerification => _database.GetCollection<EmailVerification>("PendingEmailVerification");
        public IMongoCollection<Permission> Permissions => _database.GetCollection<Permission>("Permission");
        public IMongoCollection<PermissionRole> PermissionRoles => _database.GetCollection<PermissionRole>("PermissionRole");
        public IMongoCollection<Role> Roles => _database.GetCollection<Role>("Role");
        public IMongoCollection<UseDigitalWallet> UseDigitalWallets => _database.GetCollection<UseDigitalWallet>("UseDigitalWallet");
        public IMongoCollection<User> Users => _database.GetCollection<User>("User");
    }
}
