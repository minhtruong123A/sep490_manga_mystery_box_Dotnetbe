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
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        private readonly IMongoCollection<Subscription> _subscriptionCollection;

        public SubscriptionRepository(MongoDbContext context) : base(context.GetCollection<Subscription>("Subscription"))
        {
            _subscriptionCollection = context.GetCollection<Subscription>("Subscription");
        }

        public async Task<List<Subscription>> GetAllFollowerOfUserAsync(string userId)
        {
            var subscriptions = await _subscriptionCollection.Find(x => x.UserId.Equals(userId)).ToListAsync();

            return subscriptions;
        }

        public async Task<List<Subscription>> GetAllFollowOfUserAsync(string userId)
        {
            var subscriptions = await _subscriptionCollection.Find(x => x.FollowerId.Equals(userId)).ToListAsync();
            return subscriptions;
        }
    }
}
