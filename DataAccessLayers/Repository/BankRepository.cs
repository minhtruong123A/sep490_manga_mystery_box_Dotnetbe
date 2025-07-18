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
    public class BankRepository : GenericRepository<Bank>, IBankRepository
    {
        private readonly IMongoCollection<Bank> _bankCollection;
        public BankRepository(MongoDbContext context) : base(context.GetCollection<Bank>("Bank"))
        {
            _bankCollection = context.GetCollection<Bank>("Bank");
        }

        public async Task<Bank> GetBankByAbbreviation(string abbreviation)
        {
            return await _bankCollection.Find(x => x.Abbreviation.Equals(abbreviation)).FirstOrDefaultAsync();
        }
    }
}
