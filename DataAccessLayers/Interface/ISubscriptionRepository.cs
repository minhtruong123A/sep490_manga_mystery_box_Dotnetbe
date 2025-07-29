using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface ISubscriptionRepository : IGenericRepository<Subscription>
    {
        Task<List<Subscription>> GetAllFollowerOfUserAsync(string userId);
        Task<List<Subscription>> GetAllFollowOfUserAsync(string userId);
    }
}
