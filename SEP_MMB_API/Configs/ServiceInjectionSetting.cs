using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using DataAccessLayers.UnitOfWork;
using Services.AutoMapper;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Configs;

public static class ServiceInjectionSetting
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
        services.AddScoped<IMangaBoxRepository, MangaBoxRepository>();
        services.AddScoped<ISellProductRepository, SellProductRepository>();
        services.AddScoped<IUserCollectionRepository, UserCollectionRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ITransactionHistoryRepository, TransactionHistoryRepository>();
        services.AddScoped<IUseDigitalWalletRepository, UseDigitalWalletRepository>();
        services.AddScoped<IPayOSRepository, PayOSRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IProductInMangaBoxRepository, ProductInMangaBoxRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IMysteryBoxRepository, MysteryBoxRepository>();
        services.AddScoped<IBoxOrderRepository, BoxOrderRepository>();
        services.AddScoped<IProductOrderRepository, ProductOrderRepository>();
        services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
        services.AddScoped<IDigitalPaymentSessionRepository, DigitalPaymentSessionRepository>();
        services.AddScoped<IUserBoxRepository, UserBoxRepository>();
        services.AddScoped<IUserProductRepository, UserProductRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IExchangeRepository, ExchangeRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<IUserBankRepository, UserBankRepository>();
        services.AddScoped<IBankRepository, BankRepository>();
        services.AddScoped<IExchangeSessionRepository, ExchangeSessionRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IAuctionResultRepository, AuctionResultRepository>();
        services.AddScoped<IAuctionPaymentSessionRepository, AuctionPaymentSessionRepository>();
        services.AddScoped<ITransactionFeeRepository, TransactionFeeRepository>();
        services.AddScoped<IRarityRepository, RarityRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IRewardRepository, RewardRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
        services.AddScoped<IProductFavoriteRepository, ProductFavoriteRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMangaBoxService, MangaBoxService>();
        services.AddScoped<ISellProductService, SellProductService>();
        services.AddScoped<IUserCollectionService, UserCollectionService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IModerationService, ModerationService>();
        services.AddScoped<IPayOSService, PayOSService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ITransactionHistoryService, TransactionHistoryService>();
        services.AddScoped<IUseDigitalWalletService, UseDigitalWalletService>();
        services.AddScoped<ISignedUrlService, SignedUrlService>();
        services.AddScoped<IProductInMangaBoxService, ProductInMangaBoxService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IOrderHistoryService, OrderHistoryService>();
        services.AddScoped<IUserBoxService, UserBoxService>();
        services.AddScoped<IUserProductService, UserProductService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IExchangeService, ExchangeService>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IMysteryBoxService, MysteryBoxService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<IAuctionSettlementService, AuctionSettlementService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IProductFavoriteService, ProductFavoriteService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<ITransactionFeeService, TransactionFeeService>();
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }
    
}