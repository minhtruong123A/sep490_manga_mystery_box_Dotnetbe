using BusinessObjects.Mongodb;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DataAccessLayers.UnitOfWork;

public class UnitOfWork(
    MongoDbContext context,
    IMongoClient mongoClient,
    IOptions<FeeSettings> feeOptions,
    IOptions<FavoritesSettings> favoritesSettings,
    IOptions<ExchangeSettings> exchangeSettings,
    IOptions<ProductPriceSettings> productPriceSettings,
    IOptions<RewardSettings> rewardSettings)
    : IUnitOfWork
{
    private readonly MongoDbContext _context = context;
    private readonly IOptions<ExchangeSettings> _exchangeSettings = exchangeSettings ?? throw new ArgumentNullException(nameof(exchangeSettings));
    private readonly IOptions<FavoritesSettings> _favoritesSettings = favoritesSettings ?? throw new ArgumentNullException(nameof(favoritesSettings));
    private readonly IOptions<FeeSettings> _feeOptions = feeOptions ?? throw new ArgumentNullException(nameof(feeOptions));
    private readonly IMongoClient _mongoClient = mongoClient;
    private readonly IOptions<ProductPriceSettings> _productPriceSettings = productPriceSettings ?? throw new ArgumentNullException(nameof(productPriceSettings));
    private readonly IOptions<RewardSettings> _rewardSettings = rewardSettings ?? throw new ArgumentNullException(nameof(rewardSettings));
    public IAchievementRepository _achievementRepository;
    public IAuctionPaymentSessionRepository _auctionPaymentSessionRepository;
    public IAuctionResultRepository _auctionResultRepository;
    public IAuctionSessionRepository _auctionSessionRepository;
    public IBankRepository _bankRepository;
    public IBoxOrderRepository _boxOrderRepository;
    public ICartRepository _cartRepository;
    public ICollectionRepository _collectionRepository;
    public ICommentRepository _commentRepository;
    public IDigitalPaymentSessionRepository _digitalPaymentSessionRepository;
    private IEmailVerificationRepository _emailVerificationRepository;
    public IExchangeRepository _exchangeRepository;
    public IExchangeSessionRepository _exchangeSessionRepository;
    public IProductFavoriteRepository _favoriteRepository;
    public IFeedbackRepository _feedbackRepository;
    private IMangaBoxRepository _mangaBoxRepository;
    public IMysteryBoxRepository _mysteryBoxRepository;
    public IOrderHistoryRepository _orderHistoryRepository;
    public IPayOSRepository _payosRepository;
    public IProductInMangaBoxRepository _productInMangaBoxRepository;
    public IProductOrderRepository _productOrderRepository;
    public IProductRepository _productRepository;
    public IRarityRepository _rarityRepository;
    public IReportRepository _reportRepository;
    public IRewardRepository _rewardRepository;
    private ISellProductRepository _sellProductRepository;
    public ISubscriptionRepository _subscriptionRepository;
    public ITransactionFeeRepository _transactionFeeRepository;
    public ITransactionHistoryRepository _transactionHistoryRepository;
    public IUseDigitalWalletRepository _useDigitalWalletRepository;
    public IUserAchievementRepository _userAchievementRepository;
    public IUserBankRepository _userBankRepository;
    public IUserBoxRepository _userBoxRepository;
    public IUserCollectionRepository _userCollectionRepository;
    public IUserProductRepository _userProductRepository;
    private IUserRepository _users;


    public IUserRepository UserRepository => _users ??= new UserRepository(_context);

    public IEmailVerificationRepository EmailVerificationRepository =>
        _emailVerificationRepository ??= new EmailVerificationRepository(_context);

    public IMangaBoxRepository MangaBoxRepository => _mangaBoxRepository ??= new MangaBoxRepository(_context);

    public ISellProductRepository SellProductRepository => _sellProductRepository ??=
        new SellProductRepository(_context, _feeOptions, _productPriceSettings, UserAchievementRepository,
            ExchangeRepository);

    public IUserCollectionRepository UserCollectionRepository => _userCollectionRepository ??=
        new UserCollectionRepository(_context, UserAchievementRepository);

    public ICommentRepository CommentRepository => _commentRepository ??= new CommentRepository(_context);
    public IPayOSRepository PayOSRepository => _payosRepository ??= new PayOSRepository(_context, _mongoClient);

    public IUseDigitalWalletRepository UseDigitalWalletRepository =>
        _useDigitalWalletRepository ??= new UseDigitalWalletRepository(_context);

    public ITransactionHistoryRepository TransactionHistoryRepository =>
        _transactionHistoryRepository ??= new TransactionHistoryRepository(_context);

    public ICartRepository CartRepository => _cartRepository ??= new CartRepository(_context);

    public IProductInMangaBoxRepository ProductInMangaBoxRepository =>
        _productInMangaBoxRepository ?? new ProductInMangaBoxRepository(_context);

    public IProductRepository ProductRepository =>
        _productRepository ??= new ProductRepository(_context, SellProductRepository);

    public IUserBoxRepository UserBoxRepository => _userBoxRepository ??=
        new UserBoxRepository(_context, MangaBoxRepository, UserAchievementRepository, ProductRepository, _rewardSettings);

    public IUserProductRepository UserProductRepository =>
        _userProductRepository ??= new UserProductRepository(_context);

    public IMysteryBoxRepository MysteryBoxRepository => _mysteryBoxRepository ??= new MysteryBoxRepository(_context);

    public IOrderHistoryRepository OrderHistoryRepository =>
        _orderHistoryRepository ??= new OrderHistoryRepository(_context);

    public IBoxOrderRepository BoxOrderRepository => _boxOrderRepository ??= new BoxOrderRepository(_context);

    public IDigitalPaymentSessionRepository DigitalPaymentSessionRepository =>
        _digitalPaymentSessionRepository ??= new DigitalPaymentSessionRepository(_context);

    public IProductOrderRepository productOrderRepository =>
        _productOrderRepository ??= new ProductOrderRepository(_context);

    public IReportRepository ReportRepository => _reportRepository ??= new ReportRepository(_context);

    public IExchangeRepository ExchangeRepository => _exchangeRepository ??=
        new ExchangeRepository(_context, _mongoClient, UserAchievementRepository, _exchangeSettings);

    public IExchangeSessionRepository ExchangeSessionRepository =>
        _exchangeSessionRepository ??= new ExchangeSessionRepository(_context);

    public ICollectionRepository CollectionRepository => _collectionRepository ??= new CollectionRepository(_context);
    public IUserBankRepository UserBankRepository => _userBankRepository ??= new UserBankRepository(_context);
    public IBankRepository BankRepository => _bankRepository ??= new BankRepository(_context);
    public IFeedbackRepository FeedbackRepository => _feedbackRepository ??= new FeedbackRepository(_context);

    public ISubscriptionRepository SubscriptionRepository =>
        _subscriptionRepository ??= new SubscriptionRepository(_context);

    public IProductFavoriteRepository ProductFavoriteRepository =>
        _favoriteRepository ??= new ProductFavoriteRepository(_context, _favoritesSettings);

    public IAuctionPaymentSessionRepository AuctionPaymentSessionRepository =>
        _auctionPaymentSessionRepository ??= new AuctionPaymentSessionRepository(_context);

    public IAuctionResultRepository AuctionResultRepository =>
        _auctionResultRepository ??= new AuctionResultRepository(_context);

    public ITransactionFeeRepository TransactionFeeRepository =>
        _transactionFeeRepository ??= new TransactionFeeRepository(_context);

    public IRarityRepository RarityRepository => _rarityRepository ??= new RarityRepository(_context);

    public IAchievementRepository AchievementRepository =>
        _achievementRepository ??= new AchievementRepository(_context);

    public IRewardRepository RewardRepository => _rewardRepository ??= new RewardRepository(_context);

    public IUserAchievementRepository UserAchievementRepository =>
        _userAchievementRepository ??= new UserAchievementRepository(_context, _rewardSettings);

    public IAuctionSessionRepository AuctionSessionRepository =>
        _auctionSessionRepository ??= new AuctionSessionRepository(_context);

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}