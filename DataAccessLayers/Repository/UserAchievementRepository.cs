using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
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
    public class UserAchievementRepository : GenericRepository<UserAchievement>, IUserAchievementRepository
    {
        private readonly IMongoCollection<UserAchievement> _userAchievementCollection;
        private readonly IMongoCollection<Achievement> _achievementCollection;
        private readonly IMongoCollection<UserReward> _userRewardCollection;
        private readonly IMongoCollection<Reward> _rewardCollection;
        private readonly IMongoCollection<UserCollection> _userCollectionCollection;
        private readonly IMongoCollection<UserProduct> _userProductCollection;
        private readonly IMongoCollection<Collection> _collectionCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<UserBox> _userBoxCollection;

        public UserAchievementRepository(MongoDbContext context) : base(context.GetCollection<UserAchievement>("UserAchievement"))
        {
            _userAchievementCollection = context.GetCollection<UserAchievement>("UserAchievement");
            _achievementCollection = context.GetCollection<Achievement>("Achievement");
            _userRewardCollection = context.GetCollection<UserReward>("UserReward");
            _rewardCollection = context.GetCollection<Reward>("Reward");
            _userCollectionCollection = context.GetCollection<UserCollection>("UserCollection");
            _userProductCollection = context.GetCollection<UserProduct>("UserProduct");
            _collectionCollection = context.GetCollection<Collection>("Collection");
            _productCollection = context.GetCollection<Product>("Product");
            _userBoxCollection = context.GetCollection<UserBox>("UserBox");
        }

        public async Task<bool> CheckAchievement(string userID)
        {
            var userCollections = await _userCollectionCollection.Find(x => x.UserId.Equals(userID)).ToListAsync();
            foreach (var userCollection in userCollections)
            {
                var collection = await _collectionCollection.Find(x => x.Id.Equals(userCollection.CollectionId)).FirstOrDefaultAsync();
                var achievement = await _achievementCollection.Find(x => x.CollectionId.Equals(collection.Id)).FirstOrDefaultAsync();
                var rewards = await _rewardCollection.Find(x=>x.AchievementId.Equals(achievement.Id)).ToListAsync();
                var countReward = rewards.Count();
                var products = await _productCollection.Find(x => x.CollectionId.Equals(collection.Id)).ToListAsync();
                var countProductOfCollection = products.Count();

                var userProducts = await _userProductCollection.Find(x=>x.CollectorId.Equals(userID) 
                                                                        && x.CollectionId.Equals(userCollection.Id))
                                                               .ToListAsync();     
                var countUserProduct = userProducts.Count();

                foreach(var reward in rewards)
                {
                    if(countUserProduct >= reward.Conditions)
                    {
                        var exist = await _userRewardCollection.Find(x=>x.RewardId.Equals(reward.Id)&&x.UserId.Equals(userID)).FirstOrDefaultAsync();
                        if (exist == null)
                        {
                            var newRewardBox = new UserBox
                            {
                                BoxId = reward.MangaBoxId,
                                Quantity = reward.Quantity_box,
                                UserId = userID,
                                UpdatedAt = DateTime.UtcNow,
                            };
                            await _userBoxCollection.InsertOneAsync(newRewardBox);

                            var newUserReward = new UserReward {RewardId = reward.Id, UserId = userID, isReceive = true };
                            await _userRewardCollection.InsertOneAsync(newUserReward);
                        }
                    }
                }

                if(countProductOfCollection == countUserProduct)
                {
                    var exist = await _userAchievementCollection.Find(x => x.AchievementId.Equals(achievement.Id) && x.UserId.Equals(userID)).FirstOrDefaultAsync();
                    if (exist == null)
                    {
                        var newUserAchivement = new UserAchievement { AchievementId = achievement.Id, UserId = userID };
                        await _userAchievementCollection.InsertOneAsync(newUserAchivement);
                    }
                }
            }
            return true;
        }

        public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalOfUserAsync(string userId)
        {
            var userRewards = await _userRewardCollection.Find(x => x.UserId.Equals(userId)).ToListAsync();
            var rewardIds = userRewards.Select(x=>x.RewardId).Distinct().ToList();
            var rewards = await _rewardCollection.Find(x=>rewardIds.Contains(x.Id) && x.Url_image!=null).ToListAsync();

            return userRewards.Select(u =>
            {
                var reward = rewards.FirstOrDefault(x => x.Id.Equals(u.RewardId));
                return new GetAchievementMedalRewardDto
                {
                    userRewardId = u.Id,
                    UrlImage = reward.Url_image,
                    isPublic = u.isPublic
                };
            }
            ).ToList();
        }

        public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalPublicOfUserAsync(string userId)
        {
            var userRewards = await _userRewardCollection.Find(x => x.UserId.Equals(userId)&& x.isPublic==true).ToListAsync();
            var rewardIds = userRewards.Select(x => x.RewardId).Distinct().ToList();
            var rewards = await _rewardCollection.Find(x => rewardIds.Contains(x.Id) && x.Url_image != null).ToListAsync();

            return userRewards.Select(u =>
            {
                var reward = rewards.FirstOrDefault(x => x.Id.Equals(u.RewardId));
                return new GetAchievementMedalRewardDto
                {
                    userRewardId = u.Id,
                    UrlImage = reward.Url_image,
                    isPublic = u.isPublic
                };
            }
            ).ToList();
        }

        public async Task<bool> ChangePublicOrPrivateOfMedalAsync(string userRewardId)
        {
            var userReward = await _userRewardCollection.Find(x => x.Id.Equals(userRewardId)).FirstOrDefaultAsync();
            if (userReward == null) throw new Exception("Medal not exist");
            if (userReward.isPublic)
            {
                var filter = Builders<UserReward>.Update.Set(x => x.isPublic, false);
                await _userRewardCollection.UpdateOneAsync(x => x.Id.Equals(userRewardId), filter);
                return true;
            }
            if (!userReward.isPublic)
            {
                var filter = Builders<UserReward>.Update.Set(x => x.isPublic, true);
                await _userRewardCollection.UpdateOneAsync(x => x.Id.Equals(userRewardId), filter);
                return true;
            }
            return false;

        }
    }
}
