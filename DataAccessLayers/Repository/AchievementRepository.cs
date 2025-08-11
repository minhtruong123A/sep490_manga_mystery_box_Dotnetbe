using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Reward;
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
        private readonly IMongoCollection<Reward> _rewardCollection;
        public AchievementRepository(MongoDbContext context) : base(context.GetCollection<Achievement>("Achievement"))
        {
            _achievementCollection = context.GetCollection<Achievement>("Achievement");
            _rewardCollection = context.GetCollection<Reward>("Reward");
        }

        public async Task<Achievement> GetAchievementByCollectionId(string collectionId)
        {
            var achievement = await _achievementCollection.Find(x=>x.CollectionId.Equals(collectionId)).FirstOrDefaultAsync();
            return achievement;
        }

        public async Task<AchievementWithAllRewardDto> GetAchiementWithRewardByCollectionIdAsync(string collectionId)
        {
            var achievement = await _achievementCollection.Find(x => x.CollectionId.Equals(collectionId)).FirstOrDefaultAsync();
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
}
