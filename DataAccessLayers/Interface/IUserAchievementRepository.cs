using BusinessObjects;
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
    }
}
