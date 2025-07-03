using BusinessObjects.Mongodb;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository
{
    public class OrderHistoryRepository : GenericRepository<OrderHistory>, IOrderHistoryRepository
    {
        public OrderHistoryRepository(MongoDbContext context) : base(context.GetCollection<OrderHistory>("OrderHistory"))
        {
        }
    }
}
