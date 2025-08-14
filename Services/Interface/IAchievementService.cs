using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Reward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAchievementService
    {
        Task<List<GetAchievementMedalRewardDto>> GetAllMedalOfUserAsync(string userId);
        Task<List<GetAchievementMedalRewardDto>> GetAllMedalPublicOfUserAsync(string userId);
        Task<AchievementWithAllRewardDto> GetAchiementWithRewardByCollectionIdAsync(string collectionId);
        Task<bool> ChangePublicOrPrivateAsync(string userRewardId);
        Task<AchievementOfUserCollectionCompletionProgressDto> GetUserCollectionCompletionProgressAsync(string userCollectionId);
        Task<bool> CreateAchievementOfCollection(string collectionId, string name_Achievement);
        Task<bool> CreateRewardOfAchievement(string collectionId, RewardCreateDto dto);
    }
}
