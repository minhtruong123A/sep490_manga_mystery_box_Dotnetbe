using BusinessObjects;
using BusinessObjects.Dtos.Feedback;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class FeedbackService : IFeedbackService
    { 
        private readonly IUnitOfWork _unitOfWork;

        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateFeedbackAsync(string userId,FeedbackCreateDto dto)
        {
            
            var exchange = await _unitOfWork.ExchangeRepository.GetExchangeInfoById(dto.Exchange_infoId);
            var sellproduct = await _unitOfWork.SellProductRepository.GetByIdAsync(exchange.ItemReciveId);
            var seller = await _unitOfWork.UserRepository.GetByIdAsync(sellproduct.SellerId);
            var session = await _unitOfWork.ExchangeSessionRepository.GetByIdAsync(exchange.ItemGiveId);
            if (session == null) throw new Exception("Session not existed");
            if (seller.Id.Equals(userId)) throw new Exception("You don't have permision feedback for this exchange");
            var existFeedback = await _unitOfWork.FeedbackRepository.CheckExistFeedbackAsync(exchange.BuyerId);
            if (existFeedback) throw new Exception("Feedback has been received");
            var feedback = new Feedback();
            feedback.UserId = exchange.BuyerId;
            feedback.Content = dto.Content;
            feedback.Rating = dto.Rating;
            feedback.CreateAt = DateTime.Now;
            feedback.Id = ObjectId.GenerateNewId().ToString();
            await _unitOfWork.FeedbackRepository.AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync();

            session.FeedbackId = feedback.Id;
            await _unitOfWork.ExchangeSessionRepository.UpdateAsync(session.Id, session);
            await _unitOfWork.SaveChangesAsync();

            return true;

        }

        public async Task<List<FeedbackResponeDto>> GetAllFeedbackOfProductSaleAsync(string sellProductId) => await _unitOfWork.FeedbackRepository.GetAllFeedbackOfProductSaleAsync(sellProductId);
    }
}
