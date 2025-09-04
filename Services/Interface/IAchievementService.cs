using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Reward;

namespace Services.Interface;

public interface IAchievementService
{
    Task<List<GetAchievementMedalRewardDto>> GetAllMedalOfUserAsync(string userId);
    Task<List<GetAchievementMedalRewardDto>> GetAllMedalPublicOfUserAsync(string userId);
    Task<AchievementWithAllRewardDto> GetAchiementWithRewardByCollectionIdAsync(string collectionId);
    Task<bool> ChangePublicOrPrivateAsync(string userRewardId);

    Task<List<AchievementOfUserCollectionCompletionProgressDto>>
        GetUserCollectionCompletionProgressAsync(string userId);

    Task<bool> CreateAchievementOfCollection(string collectionId, string name_Achievement);
    Task<bool> CreateRewardOfAchievement(string collectionId, RewardCreateDto dto);
}