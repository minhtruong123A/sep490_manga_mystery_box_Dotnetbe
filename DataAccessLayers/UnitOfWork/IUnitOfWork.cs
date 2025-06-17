using DataAccessLayers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IEmailVerificationRepository EmailVerificationRepository { get; }
        IMangaBoxRepository MangaBoxRepository { get; }
        ISellProductRepository SellProductRepository { get; }
        IUserCollectionRepository UserCollectionRepository { get; }
        ICommentRepository CommentRepository { get; }
        IUserBoxRepository UserBoxRepository { get; }
        IUserProductRepository UserProductRepository { get; }
        Task SaveChangesAsync();
    }
}
