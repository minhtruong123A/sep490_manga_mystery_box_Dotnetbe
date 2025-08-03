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
    public class AchievementRepository : GenericRepository<Achievement>, IAchievementRepository
    {
        private readonly IMongoCollection<Achievement> _achievementCollection;
        public AchievementRepository(MongoDbContext context) : base(context.GetCollection<Achievement>("Achievement"))
        {
            _achievementCollection = context.GetCollection<Achievement>("Achievement");
        }
    }
}
