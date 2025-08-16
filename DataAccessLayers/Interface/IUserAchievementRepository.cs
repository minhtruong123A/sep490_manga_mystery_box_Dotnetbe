using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IUserAchievementRepository : IGenericRepository<UserAchievement>
    {
        Task<bool> CheckAchievement(string userID);
        Task<List<AchievementOfUserCollectionCompletionProgressDto>> GetUserCollectionCompletionProgressAsync(string userId);
        Task<List<GetAchievementMedalRewardDto>> GetAllMedalOfUserAsync(string userId);
        Task<List<GetAchievementMedalRewardDto>> GetAllMedalPublicOfUserAsync(string userId);
        Task<bool> ChangePublicOrPrivateOfMedalAsync(string userRewardId);
    }
}
