using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using BusinessObjects.Dtos.Reward;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class AchievementService : IAchievementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly RewardSettings _settings;

        public AchievementService(IUnitOfWork unitOfWork, IImageService imageService, IOptions<RewardSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _settings = settings.Value;
        }

        public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalOfUserAsync(string userId)=> await _unitOfWork.UserAchievementRepository.GetAllMedalOfUserAsync(userId);
        public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalPublicOfUserAsync(string userId) => await _unitOfWork.UserAchievementRepository.GetAllMedalPublicOfUserAsync(userId);
        public async Task<AchievementWithAllRewardDto> GetAchiementWithRewardByCollectionIdAsync(string collectionId) => await _unitOfWork.AchievementRepository.GetAchiementWithRewardByCollectionIdAsync(collectionId);
        public async Task<bool> ChangePublicOrPrivateAsync(string userRewardId) => await _unitOfWork.UserAchievementRepository.ChangePublicOrPrivateOfMedalAsync(userRewardId);
        public async Task<AchievementOfUserCollectionCompletionProgressDto> GetUserCollectionCompletionProgressAsync(string userCollectionId) => await _unitOfWork.UserAchievementRepository.GetUserCollectionCompletionProgressAsync(userCollectionId);
        public async Task<bool> CreateAchievementOfCollection(string collectionId, string name_Achievement)
        {
            if (collectionId.Equals(_settings.UniqueRewardCollectionId)) return false;
            var achievement = await _unitOfWork.AchievementRepository.GetAchievementByCollectionId(collectionId);
            if (achievement != null) return false;
            var newAchievement = new Achievement
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = name_Achievement,
                CollectionId = collectionId,
                Create_at = DateTime.UtcNow,
            };
            await _unitOfWork.AchievementRepository.AddAsync(newAchievement);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateRewardOfAchievement(string collectionId, RewardCreateDto dto)
        {
            var achievement = await _unitOfWork.AchievementRepository.GetAchievementByCollectionId(collectionId);
            string url = null;

            if (dto.Url_image != null) url = await _imageService.UploadProfileImageAsync(dto.Url_image);

            var newReward = new Reward
            {
                AchievementId = achievement.Id,
                Conditions = dto.Conditions,
                MangaBoxId = _settings.UniqueRewardMangaBoxId,
                Quantity_box = dto.Quantity_box,
                Url_image = url,
            };
            await _unitOfWork.RewardRepository.AddAsync(newReward);
            return true;
        }
        
    }
}
