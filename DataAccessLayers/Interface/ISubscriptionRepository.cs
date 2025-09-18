using BusinessObjects;
using BusinessObjects.Dtos.Subscription;

namespace DataAccessLayers.Interface;

public interface ISubscriptionRepository : IGenericRepository<Subscription>
{
    Task<List<SubcriptionFollowerResponeDto>> GetAllFollowerOfUserAsync(string userId);
    Task<List<SubcriptionFollowResponeDto>> GetAllFollowOfUserAsync(string userId);
    Task<bool> UnfollowAsync(string userId, string followerId);
}