using BusinessObjects;
using BusinessObjects.Dtos.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISubscriptionService
    {
        Task<bool> CreateAsync(string id, SubscriptionCreateDto dto);
        Task<List<SubcriptionFollowResponeDto>> GetAllFollowOfUserAsync(string userId);
        Task<List<SubcriptionFollowerResponeDto>> GetAllFollowerOfUserAsync(string userId);
    }
}
