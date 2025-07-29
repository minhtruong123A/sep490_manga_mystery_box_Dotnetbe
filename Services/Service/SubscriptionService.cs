using BusinessObjects;
using BusinessObjects.Dtos.Subscription;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateAsync(string id,SubscriptionCreateDto dto)
        {
            var subscription = new Subscription { FollowerId = id, UserId = dto.UserId, Follow_at = DateTime.Now };
            await _unitOfWork.SubscriptionRepository.AddAsync(subscription);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<Subscription>> GetAllFollowOfUserAsync(string userId)=> await _unitOfWork.SubscriptionRepository.GetAllFollowOfUserAsync(userId);

        public async Task<List<Subscription>> GetAllFollowerOfUserAsync(string userId) => await _unitOfWork.SubscriptionRepository.GetAllFollowerOfUserAsync(userId);
    }
}
