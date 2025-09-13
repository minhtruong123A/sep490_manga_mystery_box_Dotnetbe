using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class BankRepository(MongoDbContext context)
    : GenericRepository<Bank>(context.GetCollection<Bank>("Bank")), IBankRepository
{
    private readonly IMongoCollection<Bank> _bankCollection = context.GetCollection<Bank>("Bank");

    public async Task<Bank> GetBankByAbbreviation(string abbreviation)
    {
        return await _bankCollection.Find(x => x.Abbreviation.Equals(abbreviation)).FirstOrDefaultAsync();
    }
}