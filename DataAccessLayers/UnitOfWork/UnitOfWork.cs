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

        public IUserRepository UserRepository => _users ??= new UserRepository(_context);
        public IEmailVerificationRepository EmailVerificationRepository => _emailVerificationRepository ??= new EmailVerificationRepository(_context);

        public UnitOfWork(MongoDbContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync() =>  Task.CompletedTask;
    }
}
