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
    public class RewardRepository : GenericRepository<Reward>, IRewardRepository
    {
        private readonly IMongoCollection<Reward> _rewardCollection;
        public RewardRepository(MongoDbContext context) : base(context.GetCollection<Reward>("Reward"))
        {
            _rewardCollection = context.GetCollection<Reward>("Reward");
        }
    }
}
