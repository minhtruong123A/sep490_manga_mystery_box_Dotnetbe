using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Configuration;

namespace BusinessObjects.Mongodb
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
        public IMongoCollection<Rarity> Rarities => _database.GetCollection<Rarity>("Rarity");
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Product");
        public IMongoCollection<Collection> Collections => _database.GetCollection<Collection>("Collection");
        public IMongoCollection<MysteryBox> MysteryBoxes => _database.GetCollection<MysteryBox>("MysteryBox");
        public IMongoCollection<MangaBox> MangaBoxes => _database.GetCollection<MangaBox>("MangaBox");
        public IMongoCollection<ProductInMangaBox> ProductInMangaBoxes => _database.GetCollection<ProductInMangaBox>("ProductInMangaBox");
        public IMongoCollection<UserBox> UserBoxes => _database.GetCollection<UserBox>("UserBox");
        public IMongoCollection<UserCollection> UserCollections => _database.GetCollection<UserCollection>("UserCollection");
        public IMongoCollection<UserProduct> UserProducts => _database.GetCollection<UserProduct>("User_Product");
        public IMongoCollection<SellProduct> SellProducts => _database.GetCollection<SellProduct>("SellProduct");
        public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("Comment");
        public IMongoCollection<TransactionHistory> TransactionHistories => _database.GetCollection<TransactionHistory>("TransactionHistory");
        public IMongoCollection<Cart> Carts => _database.GetCollection<Cart>("Cart");
        public IMongoCollection<CartBox> CartBoxes => _database.GetCollection<CartBox>("CartBox");
        public IMongoCollection<CartProduct> CartProducts => _database.GetCollection<CartProduct>("CartProduct");
        public IMongoCollection<BoxOrder> BoxOrders => _database.GetCollection<BoxOrder>("BoxOrder");
        public IMongoCollection<ProductOrder> ProductOrders => _database.GetCollection<ProductOrder>("ProductOrder");
        public IMongoCollection<OrderHistory> OrderHistories => _database.GetCollection<OrderHistory>("OrderHistory");
        public IMongoCollection<DigitalPaymentSession> DigitalPaymentSessions => _database.GetCollection<DigitalPaymentSession>("DigitalPaymentSession");
        public IMongoCollection<Report> Report => _database.GetCollection<Report>("Report");
        public IMongoCollection<TransactionFee> TransactionFees => _database.GetCollection<TransactionFee>("TransactionFee");
        public IMongoCollection<ExchangeInfo> ExchangeInfos => _database.GetCollection<ExchangeInfo>("ExchangeInfo");
        public IMongoCollection<ExchangeProduct> ExchangeProducts => _database.GetCollection<ExchangeProduct>("ExchangeProduct");
        public IMongoCollection<ExchangeSession> ExchangeSessiones => _database.GetCollection<ExchangeSession>("ExchangeSession");
        public IMongoCollection<UserBank> UserBanks => _database.GetCollection<UserBank>("UserBank");
        public IMongoCollection<Bank> Banks => _database.GetCollection<Bank>("Bank");
        public IMongoCollection<Feedback> Feedbakcs => _database.GetCollection<Feedback>("Feedback");
        public IMongoCollection<Subscription> Subscriptions => _database.GetCollection<Subscription>("Subscription");
        public IMongoCollection<ProductFavorite> ProductFavorites => _database.GetCollection<ProductFavorite>("ProductFavorite");
        public IMongoCollection<AuctionPaymentSession> AuctionPaymentSession => _database.GetCollection<AuctionPaymentSession>("AuctionPaymentSession");
        public IMongoCollection<AuctionResult> AuctionResult => _database.GetCollection<AuctionResult>("AuctionResult");
    }
}
