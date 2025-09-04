using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Reward;
using BusinessObjects.Mongodb;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class UserAchievementRepository(MongoDbContext context, IOptions<RewardSettings> settings)
    : GenericRepository<UserAchievement>(context.GetCollection<UserAchievement>("UserAchievement")),
        IUserAchievementRepository
{
    private readonly IMongoCollection<Achievement> _achievementCollection = context.GetCollection<Achievement>("Achievement");
    private readonly IMongoCollection<Collection> _collectionCollection = context.GetCollection<Collection>("Collection");
    private readonly IMongoCollection<MangaBox> _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
    private readonly IMongoCollection<MysteryBox> _mysteryBoxCollection = context.GetCollection<MysteryBox>("MysteryBox");
    private readonly IMongoCollection<Product> _productCollection = context.GetCollection<Product>("Product");
    private readonly IMongoCollection<Reward> _rewardCollection = context.GetCollection<Reward>("Reward");
    private readonly RewardSettings _settings = settings.Value;
    private readonly IMongoCollection<UserAchievement> _userAchievementCollection = context.GetCollection<UserAchievement>("UserAchievement");
    private readonly IMongoCollection<UserBox> _userBoxCollection = context.GetCollection<UserBox>("UserBox");
    private readonly IMongoCollection<UserCollection> _userCollectionCollection = context.GetCollection<UserCollection>("UserCollection");
    private readonly IMongoCollection<UserProduct> _userProductCollection = context.GetCollection<UserProduct>("User_Product");
    private readonly IMongoCollection<UserReward> _userRewardCollection = context.GetCollection<UserReward>("UserReward");

    public async Task<bool> CheckAchievement(string userID)
    {
        var userCollections = await _userCollectionCollection.Find(x => x.UserId.Equals(userID)).ToListAsync();
        foreach (var userCollection in userCollections)
        {
            var collection = await _collectionCollection.Find(x => x.Id.Equals(userCollection.CollectionId))
                .FirstOrDefaultAsync();
            var achievement = await _achievementCollection.Find(x => x.CollectionId.Equals(collection.Id))
                .FirstOrDefaultAsync();
            if (achievement != null)
            {
                var rewards = await _rewardCollection.Find(x => x.AchievementId.Equals(achievement.Id)).ToListAsync();
                var countReward = rewards.Count();
                var products = await _productCollection.Find(x => x.CollectionId.Equals(collection.Id)).ToListAsync();
                var countProductOfCollection = products.Count();

                var userProducts = await _userProductCollection.Find(x => x.CollectorId.Equals(userID)
                                                                          && x.CollectionId.Equals(userCollection.Id))
                    .ToListAsync();
                var countUserProduct = userProducts.Count();

                foreach (var reward in rewards)
                    if (countUserProduct >= reward.Conditions)
                    {
                        var exist = await _userRewardCollection
                            .Find(x => x.RewardId.Equals(reward.Id) && x.UserId.Equals(userID)).FirstOrDefaultAsync();
                        if (exist == null)
                        {
                            var rewardBoxExist = await _userBoxCollection
                                .Find(x => x.BoxId.Equals(_settings.UniqueRewardMangaBoxId)).FirstOrDefaultAsync();
                            if (rewardBoxExist == null)
                            {
                                var newRewardBox = new UserBox
                                {
                                    BoxId = reward.MangaBoxId,
                                    Quantity = reward.Quantity_box,
                                    UserId = userID,
                                    UpdatedAt = DateTime.UtcNow
                                };
                                await _userBoxCollection.InsertOneAsync(newRewardBox);
                            }

                            //var updateQuantity = Builders<UserBox>.Update.Inc(x => x.Quantity, reward.Quantity_box);
                            //Console.WriteLine("ưefjiowfjojifeow" + rewardBoxExist.Id);
                            //await _userBoxCollection.UpdateOneAsync(rewardBoxExist.Id, updateQuantity);
                            var filter = Builders<UserBox>.Filter.Eq(x => x.Id, rewardBoxExist.Id);
                            var updateQuantity = Builders<UserBox>.Update.Inc(x => x.Quantity, reward.Quantity_box);
                            await _userBoxCollection.UpdateOneAsync(filter, updateQuantity);

                            var newUserReward = new UserReward
                                { RewardId = reward.Id, UserId = userID, isReceive = true };
                            await _userRewardCollection.InsertOneAsync(newUserReward);
                        }
                    }

                if (countProductOfCollection == countUserProduct)
                {
                    var exist = await _userAchievementCollection
                        .Find(x => x.AchievementId.Equals(achievement.Id) && x.UserId.Equals(userID))
                        .FirstOrDefaultAsync();
                    if (exist == null)
                    {
                        var newUserAchivement = new UserAchievement { AchievementId = achievement.Id, UserId = userID };
                        await _userAchievementCollection.InsertOneAsync(newUserAchivement);
                    }
                }
            }
        }

        return true;
    }

    public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalOfUserAsync(string userId)
    {
        var userRewards = await _userRewardCollection.Find(x => x.UserId.Equals(userId)).ToListAsync();
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

    public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalPublicOfUserAsync(string userId)
    {
        var userRewards = await _userRewardCollection.Find(x => x.UserId.Equals(userId) && x.isPublic == true)
            .ToListAsync();
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

    public async Task<List<AchievementOfUserCollectionCompletionProgressDto>>
        GetUserCollectionCompletionProgressAsync(string userId)
    {
        var userCollections = await _userCollectionCollection.Find(x => x.UserId.Equals(userId)).ToListAsync();
        var userCollectionIds = userCollections.Select(x => x.CollectionId).Distinct().ToList();

        var achievements = await _achievementCollection.Find(x => userCollectionIds.Contains(x.CollectionId))
            .ToListAsync();
        var achievementIds = achievements.Select(x => x.Id).Distinct().ToList();

        var userRewards = await _userRewardCollection.Find(x => x.UserId.Equals(userId)).ToListAsync();
        var rewards = await _rewardCollection.Find(x => achievementIds.Contains(x.AchievementId)).ToListAsync();

        var mangaBox = await _mangaBoxCollection.Find(x => x.Id.Equals(_settings.UniqueRewardMangaBoxId))
            .FirstOrDefaultAsync();
        var mysBox = await _mysteryBoxCollection.Find(x => x.Id.Equals(mangaBox.MysteryBoxId)).FirstOrDefaultAsync();

        return achievements.Select(a =>
        {
            var rewardDtos = rewards.Select(r =>
            {
                if (r.AchievementId.Equals(a.Id))
                {
                    var existComplete = userRewards.FirstOrDefault(x => x.RewardId.Equals(r.Id));
                    if (existComplete != null)
                        return new ReawrdCompletionProgressOfUserCollectionDto
                        {
                            AchievementId = a.Id,
                            Conditions = r.Conditions,
                            MangaBoxId = r.MangaBoxId,
                            Quantity_box = r.Quantity_box,
                            Url_image = r.Url_image ?? "",
                            MangaBox_image = mysBox?.UrlImage ?? "",
                            isComplete = true
                        };
                    return new ReawrdCompletionProgressOfUserCollectionDto
                    {
                        AchievementId = a.Id,
                        Conditions = r.Conditions,
                        MangaBoxId = r.MangaBoxId,
                        Quantity_box = r.Quantity_box,
                        Url_image = r.Url_image ?? "",
                        MangaBox_image = mysBox?.UrlImage ?? "",
                        isComplete = false
                    };
                }

                return new ReawrdCompletionProgressOfUserCollectionDto();
            }).ToList();

            var collection = _collectionCollection.Find(x => x.Id.Equals(a.CollectionId)).FirstOrDefault();
            var userCollection = userCollections.FirstOrDefault(x => x.CollectionId.Equals(collection.Id));
            var countUserProducts = _userProductCollection.Find(x => x.CollectionId.Equals(userCollection.Id)).ToList()
                .Count();
            return new AchievementOfUserCollectionCompletionProgressDto
            {
                Id = a.Id,
                CollectionId = a.CollectionId,
                AchievementName = a.Name,
                CollectionName = collection.Topic,
                Count = countUserProducts,
                dtos = rewardDtos.Where(x => x.AchievementId != null).ToList()
            };
        }).ToList();
    }
}