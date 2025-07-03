using BusinessObjects.Mongodb;
using BusinessObjects;
using DataAccessLayers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class DigitalPaymentSessionRepository : GenericRepository<DigitalPaymentSession>, IDigitalPaymentSessionRepository
    {
        public DigitalPaymentSessionRepository(MongoDbContext context) : base(context.GetCollection<DigitalPaymentSession>("DigitalPaymentSession"))
        {
        }
    }
}
