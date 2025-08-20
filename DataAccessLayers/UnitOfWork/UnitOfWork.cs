using BusinessObjects.Mongodb;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MongoDbContext _context;
        private readonly IOptions<FeeSettings> _feeOptions;
        private readonly IOptions<FavoritesSettings> _favoritesSettings;
        private readonly IOptions<ExchangeSettings> _exchangeSettings;
        private readonly IOptions<ProductPriceSettings> _productPriceSettings;
        private readonly IOptions<RewardSettings> _rewardSettings;
        private readonly IMongoClient _mongoClient;
        private IUserRepository _users;
        private IEmailVerificationRepository _emailVerificationRepository;
        private IMangaBoxRepository _mangaBoxRepository;
        private ISellProductRepository _sellProductRepository;
        public IUserCollectionRepository _userCollectionRepository;
        public IUserBoxRepository _userBoxRepository;
        public ICommentRepository _commentRepository;
        public IPayOSRepository _payosRepository;
        public IUseDigitalWalletRepository _useDigitalWalletRepository;
        public ITransactionHistoryRepository _transactionHistoryRepository;
        public ICartRepository _cartRepository;
        public IProductInMangaBoxRepository _productInMangaBoxRepository;
        public IProductRepository _productRepository;
        public IUserProductRepository _userProductRepository;
        public IMysteryBoxRepository _mysteryBoxRepository;
        public IOrderHistoryRepository _orderHistoryRepository;
        public IBoxOrderRepository _boxOrderRepository;
        public IDigitalPaymentSessionRepository _digitalPaymentSessionRepository;
        public IProductOrderRepository _productOrderRepository;
        public IReportRepository _reportRepository;
        public IExchangeRepository _exchangeRepository;
        public IExchangeSessionRepository _exchangeSessionRepository;
        public ICollectionRepository _collectionRepository;
        public IUserBankRepository _userBankRepository;
        public IBankRepository _bankRepository;
        public IFeedbackRepository _feedbackRepository;
        public ISubscriptionRepository _subscriptionRepository;
        public IProductFavoriteRepository _favoriteRepository;
        public IAuctionPaymentSessionRepository _auctionPaymentSessionRepository;
        public IAuctionResultRepository _auctionResultRepository;
        public ITransactionFeeRepository _transactionFeeRepository;
        public IRarityRepository _rarityRepository;
        public IAchievementRepository _achievementRepository;
        public IRewardRepository _rewardRepository;
        public IUserAchievementRepository _userAchievementRepository;
        public IAuctionSessionRepository _auctionSessionRepository;



        public IUserRepository UserRepository => _users ??= new UserRepository(_context);
        public IEmailVerificationRepository EmailVerificationRepository => _emailVerificationRepository ??= new EmailVerificationRepository(_context);
        public IMangaBoxRepository MangaBoxRepository => _mangaBoxRepository ??= new MangaBoxRepository(_context);
        public ISellProductRepository SellProductRepository => _sellProductRepository ??= new SellProductRepository(_context, _feeOptions, _productPriceSettings, UserAchievementRepository, ExchangeRepository);
        public IUserCollectionRepository UserCollectionRepository => _userCollectionRepository ??= new UserCollectionRepository(_context, UserAchievementRepository);
        public ICommentRepository CommentRepository => _commentRepository ??= new CommentRepository(_context);
        public IPayOSRepository PayOSRepository => _payosRepository ??= new PayOSRepository(_context, _mongoClient);
        public IUseDigitalWalletRepository UseDigitalWalletRepository => _useDigitalWalletRepository ??= new UseDigitalWalletRepository(_context);
        public ITransactionHistoryRepository TransactionHistoryRepository => _transactionHistoryRepository ??= new TransactionHistoryRepository(_context);
        public ICartRepository CartRepository => _cartRepository ??= new CartRepository(_context);
        public IProductInMangaBoxRepository ProductInMangaBoxRepository => _productInMangaBoxRepository ?? new ProductInMangaBoxRepository(_context);
        public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(_context);
        public IUserBoxRepository UserBoxRepository => _userBoxRepository ??= new UserBoxRepository(_context, MangaBoxRepository, UserAchievementRepository);
        public IUserProductRepository UserProductRepository => _userProductRepository ??= new UserProductRepository(_context);
        public IMysteryBoxRepository MysteryBoxRepository => _mysteryBoxRepository ??= new MysteryBoxRepository(_context);
        public IOrderHistoryRepository OrderHistoryRepository => _orderHistoryRepository ??= new OrderHistoryRepository(_context);
        public IBoxOrderRepository BoxOrderRepository => _boxOrderRepository ??= new BoxOrderRepository(_context);
        public IDigitalPaymentSessionRepository DigitalPaymentSessionRepository => _digitalPaymentSessionRepository ??= new DigitalPaymentSessionRepository(_context);
        public IProductOrderRepository productOrderRepository => _productOrderRepository ??= new ProductOrderRepository(_context);
        public IReportRepository ReportRepository => _reportRepository ??= new ReportRepository(_context);
        public IExchangeRepository ExchangeRepository => _exchangeRepository ??= new ExchangeRepository(_context, _mongoClient, UserAchievementRepository, _exchangeSettings);
        public IExchangeSessionRepository ExchangeSessionRepository => _exchangeSessionRepository ??= new ExchangeSessionRepository(_context);
        public ICollectionRepository CollectionRepository => _collectionRepository ??= new CollectionRepository(_context);
        public IUserBankRepository UserBankRepository => _userBankRepository ??= new UserBankRepository(_context);
        public IBankRepository BankRepository => _bankRepository ??= new BankRepository(_context);
        public IFeedbackRepository FeedbackRepository => _feedbackRepository ??= new FeedbackRepository(_context);
        public ISubscriptionRepository SubscriptionRepository => _subscriptionRepository ??= new SubscriptionRepository(_context);
        public IProductFavoriteRepository ProductFavoriteRepository => _favoriteRepository ??= new ProductFavoriteRepository(_context, _favoritesSettings);
        public IAuctionPaymentSessionRepository AuctionPaymentSessionRepository => _auctionPaymentSessionRepository ??= new AuctionPaymentSessionRepository(_context);
        public IAuctionResultRepository AuctionResultRepository => _auctionResultRepository ??= new AuctionResultRepository(_context);
        public ITransactionFeeRepository TransactionFeeRepository => _transactionFeeRepository ??= new TransactionFeeRepository(_context);
        public IRarityRepository RarityRepository => _rarityRepository ??= new RarityRepository(_context);
        public IAchievementRepository AchievementRepository => _achievementRepository ??= new AchievementRepository(_context);
        public IRewardRepository RewardRepository => _rewardRepository ??= new RewardRepository(_context);
        public IUserAchievementRepository UserAchievementRepository => _userAchievementRepository ??= new UserAchievementRepository(_context, _rewardSettings);
        public IAuctionSessionRepository AuctionSessionRepository => _auctionSessionRepository ??= new AuctionSessionRepository(_context);


        public UnitOfWork(MongoDbContext context, IMongoClient mongoClient, IOptions<FeeSettings> feeOptions, IOptions<FavoritesSettings> favoritesSettings, IOptions<ExchangeSettings> exchangeSettings, IOptions<ProductPriceSettings> productPriceSettings, IOptions<RewardSettings> rewardSettings)
        {
            _context = context;
            _mongoClient = mongoClient;
            _feeOptions = feeOptions ?? throw new ArgumentNullException(nameof(feeOptions));
            _favoritesSettings = favoritesSettings ?? throw new ArgumentNullException(nameof(favoritesSettings));
            _exchangeSettings = exchangeSettings ?? throw new ArgumentNullException(nameof(exchangeSettings));
            _productPriceSettings = productPriceSettings ?? throw new ArgumentNullException(nameof(productPriceSettings));
            _rewardSettings = rewardSettings ?? throw new ArgumentNullException(nameof(rewardSettings));
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
