using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class ExchangeSessionRepository : GenericRepository<ExchangeSession>, IExchangeSessionRepository
    {
        public ExchangeSessionRepository(MongoDbContext context) : base(context.GetCollection<ExchangeSession>("ExchangeSession"))
        {
        }
    }
}
