﻿using DataAccessLayers.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IEmailVerificationRepository EmailVerificationRepository { get; }
        IMangaBoxRepository MangaBoxRepository { get; }
        ISellProductRepository SellProductRepository { get; }
        IUserCollectionRepository UserCollectionRepository { get; }
        ICommentRepository CommentRepository { get; }
        ITransactionHistoryRepository TransactionHistoryRepository { get; }
        IPayOSRepository PayOSRepository { get; }
        IUseDigitalWalletRepository UseDigitalWalletRepository { get; }
        ICartRepository CartRepository { get; }
        IProductInMangaBoxRepository ProductInMangaBoxRepository { get; }
        IProductRepository ProductRepository { get; }
        IUserBoxRepository UserBoxRepository { get; }
        IUserProductRepository UserProductRepository { get; }
        IMysteryBoxRepository MysteryBoxRepository { get; }
        IOrderHistoryRepository OrderHistoryRepository { get; }
        IBoxOrderRepository BoxOrderRepository { get; }
        IDigitalPaymentSessionRepository DigitalPaymentSessionRepository { get; }
        IProductOrderRepository productOrderRepository { get; }
        IReportRepository ReportRepository { get; }
        IExchangeRepository ExchangeRepository { get; }
        ICollectionRepository CollectionRepository { get; }
        IUserBankRepository UserBankRepository { get; }
        IBankRepository BankRepository { get; }
        Task SaveChangesAsync();
    }
}
