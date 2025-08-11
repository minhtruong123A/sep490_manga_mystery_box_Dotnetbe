using BusinessObjects;
using BusinessObjects.Dtos.Achievement;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class AchievementService : IAchievementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public AchievementService(IUnitOfWork unitOfWork,IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalOfUserAsync(string userId)=> await _unitOfWork.UserAchievementRepository.GetAllMedalOfUserAsync(userId);
        public async Task<List<GetAchievementMedalRewardDto>> GetAllMedalPublicOfUserAsync(string userId) => await _unitOfWork.UserAchievementRepository.GetAllMedalPublicOfUserAsync(userId);
        public async Task<bool> ChangePublicOrPrivateAsync(string userRewardId) => await _unitOfWork.UserAchievementRepository.ChangePublicOrPrivateOfMedalAsync(userRewardId);
        public async Task<bool> CreateAchievementWithRewardOfCollection(string collectionId, AchievementWithRewardsCreateDto dto)
        {
            if(collectionId.Equals("689874ca303a71b2024bcda4")) return false;
            var aId = ObjectId.GenerateNewId().ToString();
            var achievement = new Achievement
            {
                Id = aId,
                Name = dto.Name_Achievement,
                CollectionId = collectionId,
                Create_at = DateTime.UtcNow,
            };
            await _unitOfWork.AchievementRepository.AddAsync(achievement);
            await _unitOfWork.SaveChangesAsync();

            foreach (var reward in dto.dtos)
            {
                string url = null;
                if (reward != null) url = await _imageService.UploadModeratorProductOrMysteryBoxImageAsync(reward.Url_image);

                var newReward = new Reward
                {
                    AchievementId = aId,
                    Conditions = reward.Conditions,
                    MangaBoxId = "6899caa6250b50c9837a4aec",
                    Quantity_box = reward.Quantity_box,
                    Url_image = url,
                };
                await _unitOfWork.RewardRepository.AddAsync(newReward);
            }
            return true;
        }
    }
}
