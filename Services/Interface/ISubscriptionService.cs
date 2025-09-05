using BusinessObjects.Dtos.Subscription;

namespace Services.Interface;

public interface ISubscriptionService
{
    Task<bool> CreateAsync(string id, SubscriptionCreateDto dto);
    Task<List<SubcriptionFollowResponeDto>> GetAllFollowOfUserAsync(string userId);
    Task<List<SubcriptionFollowerResponeDto>> GetAllFollowerOfUserAsync(string userId);
    Task<bool> UnfollowAsync(string userId, string followerId);
}