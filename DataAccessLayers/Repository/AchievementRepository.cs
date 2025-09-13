using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Reward;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class AchievementRepository(MongoDbContext context)
    : GenericRepository<Achievement>(context.GetCollection<Achievement>("Achievement")), IAchievementRepository
{
    private readonly IMongoCollection<Achievement> _achievementCollection = context.GetCollection<Achievement>("Achievement");
    private readonly IMongoCollection<Reward> _rewardCollection = context.GetCollection<Reward>("Reward");

    public async Task<Achievement?> GetAchievementByCollectionId(string collectionId)
    {
        var achievement = await _achievementCollection.Find(x => x.CollectionId.Equals(collectionId))
            .FirstOrDefaultAsync();
        return achievement;
    }

    public async Task<AchievementWithAllRewardDto> GetAchiementWithRewardByCollectionIdAsync(string collectionId)
    {
        var achievement = await _achievementCollection.Find(x => x.CollectionId.Equals(collectionId))
            .FirstOrDefaultAsync();
        var rewards = await _rewardCollection.Find(x => x.AchievementId.Equals(achievement.Id)).ToListAsync();
        var rewardDtos = rewards.Select(r =>
        {
            return new RewardGetDto
            {
                Conditions = r.Conditions,
                MangaBoxId = r.MangaBoxId,
                Quantity_box = r.Quantity_box,
                Url_image = r.Url_image ?? ""
            };
        }).ToList();
        return new AchievementWithAllRewardDto
        {
            Id = achievement.Id,
            CollectionId = achievement.CollectionId,
            Create_at = achievement.Create_at,
            Name = achievement.Name,
            dtos = rewardDtos
        };
    }
}