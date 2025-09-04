using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Reward;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Services.Interface;

namespace Services.Service;

public class AchievementService(IUnitOfWork unitOfWork, IImageService imageService, IOptions<RewardSettings> settings)
    : IAchievementService
{
    private readonly RewardSettings _settings = settings.Value;

    public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalOfUserAsync(string userId)
    {
        return await unitOfWork.UserAchievementRepository.GetAllMedalOfUserAsync(userId);
    }

    public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalPublicOfUserAsync(string userId)
    {
        return await unitOfWork.UserAchievementRepository.GetAllMedalPublicOfUserAsync(userId);
    }

    public async Task<AchievementWithAllRewardDto> GetAchiementWithRewardByCollectionIdAsync(string collectionId)
    {
        return await unitOfWork.AchievementRepository.GetAchiementWithRewardByCollectionIdAsync(collectionId);
    }

    public async Task<bool> ChangePublicOrPrivateAsync(string userRewardId)
    {
        return await unitOfWork.UserAchievementRepository.ChangePublicOrPrivateOfMedalAsync(userRewardId);
    }

    public async Task<List<AchievementOfUserCollectionCompletionProgressDto>>
        GetUserCollectionCompletionProgressAsync(string userId)
    {
        return await unitOfWork.UserAchievementRepository.GetUserCollectionCompletionProgressAsync(userId);
    }

    public async Task<bool> CreateAchievementOfCollection(string collectionId, string name_Achievement)
    {
        if (collectionId.Equals(_settings.UniqueRewardCollectionId)) return false;
        var achievement = await unitOfWork.AchievementRepository.GetAchievementByCollectionId(collectionId);
        if (achievement != null) return false;
        var newAchievement = new Achievement
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = name_Achievement,
            CollectionId = collectionId,
            Create_at = DateTime.UtcNow
        };
        await unitOfWork.AchievementRepository.AddAsync(newAchievement);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateRewardOfAchievement(string collectionId, RewardCreateDto dto)
    {
        var achievement = await unitOfWork.AchievementRepository.GetAchievementByCollectionId(collectionId);
        string url = null;

        if (dto.Url_image != null) url = await imageService.UploadProfileImageAsync(dto.Url_image);
        if (dto.Quantity_box == null)
        {
            var newReward1 = new Reward
            {
                AchievementId = achievement.Id,
                Conditions = dto.Conditions,
                MangaBoxId = _settings.UniqueRewardMangaBoxId,
                Quantity_box = 0,
                Url_image = url
            };
            await unitOfWork.RewardRepository.AddAsync(newReward1);
            return true;
        }

        var newReward = new Reward
        {
            AchievementId = achievement.Id,
            Conditions = dto.Conditions,
            MangaBoxId = _settings.UniqueRewardMangaBoxId,
            Quantity_box = dto.Quantity_box,
            Url_image = url
        };
        await unitOfWork.RewardRepository.AddAsync(newReward);

        return true;
    }
}