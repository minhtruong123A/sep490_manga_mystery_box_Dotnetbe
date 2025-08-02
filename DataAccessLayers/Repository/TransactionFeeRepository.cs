using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class TransactionFeeRepository : GenericRepository<TransactionFee>, ITransactionFeeRepository
    {
        private readonly IMongoCollection<TransactionFee> _transactionFeeCollection;
        public TransactionFeeRepository(MongoDbContext context) : base(context.GetCollection<TransactionFee>("TransactionFee"))
        {
            _transactionFeeCollection = context.GetCollection<TransactionFee>("TransactionFee");
        }
    }
}
