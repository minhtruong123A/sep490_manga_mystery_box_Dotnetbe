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
        public IUserProductRepository _userProductRepository;

        public IUserRepository UserRepository => _users ??= new UserRepository(_context);
        public IEmailVerificationRepository EmailVerificationRepository => _emailVerificationRepository ??= new EmailVerificationRepository(_context);
        public IMangaBoxRepository MangaBoxRepository => _mangaBoxRepository ??= new MangaBoxRepository(_context);
        public ISellProductRepository SellProductRepository => _sellProductRepository ??= new SellProductRepository(_context);
        public IUserCollectionRepository UserCollectionRepository => _userCollectionRepository ??= new UserCollectionRepository(_context);
        public ICommentRepository CommentRepository => _commentRepository ??= new CommentRepository(_context);
        public IUserBoxRepository UserBoxRepository => _userBoxRepository ??= new UserBoxRepository(_context);
        public IUserProductRepository UserProductRepository => _userProductRepository ??= new UserProductRepository(_context);


        public UnitOfWork(MongoDbContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync() =>  Task.CompletedTask;
    }
}
