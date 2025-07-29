using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
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


        public IUserRepository UserRepository => _users ??= new UserRepository(_context);
        public IEmailVerificationRepository EmailVerificationRepository => _emailVerificationRepository ??= new EmailVerificationRepository(_context);
        public IMangaBoxRepository MangaBoxRepository => _mangaBoxRepository ??= new MangaBoxRepository(_context);
        public ISellProductRepository SellProductRepository => _sellProductRepository ??= new SellProductRepository(_context);
        public IUserCollectionRepository UserCollectionRepository => _userCollectionRepository ??= new UserCollectionRepository(_context);
        public ICommentRepository CommentRepository => _commentRepository ??= new CommentRepository(_context);
        public IPayOSRepository PayOSRepository => _payosRepository ??= new PayOSRepository(_context);
        public IUseDigitalWalletRepository UseDigitalWalletRepository => _useDigitalWalletRepository ??= new UseDigitalWalletRepository(_context);
        public ITransactionHistoryRepository TransactionHistoryRepository => _transactionHistoryRepository ??= new TransactionHistoryRepository(_context);
        public ICartRepository CartRepository => _cartRepository ??= new CartRepository(_context);
        public IProductInMangaBoxRepository ProductInMangaBoxRepository => _productInMangaBoxRepository ?? new ProductInMangaBoxRepository(_context);
        public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(_context);
        public IUserBoxRepository UserBoxRepository => _userBoxRepository ??= new UserBoxRepository(_context, MangaBoxRepository);
        public IUserProductRepository UserProductRepository => _userProductRepository ??= new UserProductRepository(_context);
        public IMysteryBoxRepository MysteryBoxRepository => _mysteryBoxRepository ??= new MysteryBoxRepository(_context);
        public IOrderHistoryRepository OrderHistoryRepository => _orderHistoryRepository ??= new OrderHistoryRepository(_context);
        public IBoxOrderRepository BoxOrderRepository => _boxOrderRepository ??= new BoxOrderRepository(_context);
        public IDigitalPaymentSessionRepository DigitalPaymentSessionRepository => _digitalPaymentSessionRepository ??= new DigitalPaymentSessionRepository(_context);
        public IProductOrderRepository productOrderRepository => _productOrderRepository ??= new ProductOrderRepository(_context);
        public IReportRepository ReportRepository => _reportRepository ??= new ReportRepository(_context);
        public IExchangeRepository ExchangeRepository => _exchangeRepository ??= new ExchangeRepository(_context);
        public IExchangeSessionRepository ExchangeSessionRepository => _exchangeSessionRepository ??= new ExchangeSessionRepository(_context);
        public ICollectionRepository CollectionRepository => _collectionRepository ??= new CollectionRepository(_context);
        public IUserBankRepository UserBankRepository => _userBankRepository ??= new UserBankRepository(_context);
        public IBankRepository BankRepository => _bankRepository ??= new BankRepository(_context);
        public IFeedbackRepository FeedbackRepository => _feedbackRepository ??= new FeedbackRepository(_context);
        public ISubscriptionRepository SubscriptionRepository => _subscriptionRepository ??= new SubscriptionRepository(_context);
        public IProductFavoriteRepository ProductFavoriteRepository => _favoriteRepository ??= new ProductFavoriteRepository(_context);

        public UnitOfWork(MongoDbContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
