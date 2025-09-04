using BusinessObjects;
using BusinessObjects.Dtos.Achievement;

namespace DataAccessLayers.Interface;

public interface IAchievementRepository : IGenericRepository<Achievement>
{
    Task<Achievement?> GetAchievementByCollectionId(string collectionId);
    Task<AchievementWithAllRewardDto> GetAchiementWithRewardByCollectionIdAsync(string collectionId);
}