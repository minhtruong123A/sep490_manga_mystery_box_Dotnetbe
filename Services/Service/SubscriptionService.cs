using BusinessObjects;
using BusinessObjects.Dtos.Subscription;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class SubscriptionService(IUnitOfWork unitOfWork) : ISubscriptionService
{
    public async Task<bool> CreateAsync(string id, SubscriptionCreateDto dto)
    {
        var existUser = await unitOfWork.UserRepository.GetByIdAsync(dto.UserId);
        var follows = await unitOfWork.SubscriptionRepository.GetAllFollowOfUserAsync(id);
        var existFollow = follows.FirstOrDefault(x => x.FollowerId.Equals(id) && x.UserId.Equals(dto.UserId));
        if (existFollow != null) throw new Exception("Followed this user");
        if (existUser == null) throw new Exception("User not exist");
        var subscription = new Subscription { FollowerId = id, UserId = dto.UserId, Follow_at = DateTime.Now };
        await unitOfWork.SubscriptionRepository.AddAsync(subscription);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<SubcriptionFollowResponeDto>> GetAllFollowOfUserAsync(string userId)
    {
        return await unitOfWork.SubscriptionRepository.GetAllFollowOfUserAsync(userId);
    }

    public async Task<List<SubcriptionFollowerResponeDto>> GetAllFollowerOfUserAsync(string userId)
    {
        return await unitOfWork.SubscriptionRepository.GetAllFollowerOfUserAsync(userId);
    }

    public async Task<bool> UnfollowAsync(string userId, string followerid)
    {
        return await unitOfWork.SubscriptionRepository.UnfollowAsync(userId, followerid);
    }
}