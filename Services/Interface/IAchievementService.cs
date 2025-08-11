using BusinessObjects.Dtos.Achievement;
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
        Task<bool> ChangePublicOrPrivateAsync(string userRewardId);
    }
}
