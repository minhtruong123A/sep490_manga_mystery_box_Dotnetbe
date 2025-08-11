using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IAchievementRepository : IGenericRepository<Achievement>
    {
        Task<Achievement> GetAchievementByCollectionId(string collectionId);
        Task<AchievementWithAllRewardDto> GetAchiementWithRewardByCollectionIdAsync(string collectionId);
    }
}
