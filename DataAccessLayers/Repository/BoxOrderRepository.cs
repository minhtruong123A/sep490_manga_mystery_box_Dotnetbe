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
    public class BoxOrderRepository : GenericRepository<BoxOrder>, IBoxOrderRepository
    {
        public BoxOrderRepository(MongoDbContext context) : base(context.GetCollection<BoxOrder>("BoxOrder"))
        {
        }
    }
}
