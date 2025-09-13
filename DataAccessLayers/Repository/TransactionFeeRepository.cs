using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class TransactionFeeRepository(MongoDbContext context)
    : GenericRepository<TransactionFee>(context.GetCollection<TransactionFee>("TransactionFee")),
        ITransactionFeeRepository
{
    private readonly IMongoCollection<TransactionFee> _transactionFeeCollection = context.GetCollection<TransactionFee>("TransactionFee");
}