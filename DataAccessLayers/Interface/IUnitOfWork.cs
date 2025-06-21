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
        ITransactionHistoryRepository transactionHistoryRepository { get; }
        IPayOSRepository payOSRepository { get; }
        IUseDigitalWalletRepository useDigitalWalletRepository {  get; }
        Task SaveChangesAsync();
    }
}
