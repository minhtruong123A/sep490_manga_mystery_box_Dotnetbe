using BusinessObjects;
using BusinessObjects.Dtos.Subscription;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
{
    private readonly IMongoCollection<Subscription> _subscriptionCollection;
    private readonly IMongoCollection<User> _userCollection;

    public SubscriptionRepository(MongoDbContext context) : base(context.GetCollection<Subscription>("Subscription"))
    {
        _subscriptionCollection = context.GetCollection<Subscription>("Subscription");
        _userCollection = context.GetCollection<User>("User");
    }

    public async Task<List<SubcriptionFollowerResponeDto>> GetAllFollowerOfUserAsync(string userId)
    {
        var subscriptions = await _subscriptionCollection.Find(x => x.UserId.Equals(userId)).ToListAsync();
        var followerIds = subscriptions.Select(x => x.FollowerId).Distinct().ToList();

        var users = await _userCollection.Find(x => followerIds.Contains(x.Id)).ToListAsync();

        return subscriptions.Select(subscription =>
        {
            var user = users.FirstOrDefault(x => x.Id.Equals(subscription.FollowerId));
            return new SubcriptionFollowerResponeDto
            {
                FollowerId = subscription.FollowerId,
                UserId = userId,
                FollowerName = user.Username,
                UrlImage = user.ProfileImage,
                Follow_at = subscription.Follow_at
            };
        }).ToList();
    }

    public async Task<List<SubcriptionFollowResponeDto>> GetAllFollowOfUserAsync(string userId)
    {
        var subscriptions = await _subscriptionCollection.Find(x => x.FollowerId.Equals(userId)).ToListAsync();
        var userIds = subscriptions.Select(x => x.UserId).Distinct().ToList();

        var users = await _userCollection.Find(x => userIds.Contains(x.Id)).ToListAsync();
        return subscriptions.Select(subscription =>
        {
            var user = users.FirstOrDefault(x => x.Id.Equals(subscription.UserId));
            return new SubcriptionFollowResponeDto
            {
                FollowerId = userId,
                UserId = subscription.UserId,
                UserName = user.Username,
                UrlImage = user.ProfileImage,
                Follow_at = subscription.Follow_at
            };
        }).ToList();
    }

    public async Task<bool> UnfollowAsync(string userId, string followerId)
    {
        var subscription = await _subscriptionCollection
            .Find(x => x.UserId.Equals(userId) && x.FollowerId.Equals(followerId)).FirstOrDefaultAsync();
        if (subscription == null) throw new Exception("Subscription not exist");
        var filter = Builders<Subscription>.Filter.Eq(x => x.Id, subscription.Id);
        var result = await _subscriptionCollection.DeleteOneAsync(filter);
        return true;
    }
}