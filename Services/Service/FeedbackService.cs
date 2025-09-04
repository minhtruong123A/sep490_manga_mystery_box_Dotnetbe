using BusinessObjects;
using BusinessObjects.Dtos.Feedback;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using Services.Interface;

namespace Services.Service;

public class FeedbackService(IUnitOfWork unitOfWork) : IFeedbackService
{
    public async Task<bool> CreateFeedbackAsync(string userId, FeedbackCreateDto dto)
    {
        var exchange = await unitOfWork.ExchangeRepository.GetExchangeInfoById(dto.Exchange_infoId);
        var sellProduct = await unitOfWork.SellProductRepository.GetByIdAsync(exchange.ItemReciveId);
        var seller = await unitOfWork.UserRepository.GetByIdAsync(sellProduct.SellerId);
        var session = await unitOfWork.ExchangeSessionRepository.GetByIdAsync(exchange.ItemGiveId);
        if (session == null) throw new Exception("Session not existed");
        if (seller.Id.Equals(userId)) throw new Exception("You don't have permision feedback for this exchange");
        var existFeedback = await unitOfWork.FeedbackRepository.CheckExistFeedbackAsync(exchange.BuyerId);
        if (existFeedback) throw new Exception("Feedback has been received");
        var feedback = new Feedback();
        feedback.UserId = exchange.BuyerId;
        feedback.Content = dto.Content;
        feedback.Rating = dto.Rating;
        feedback.CreateAt = DateTime.Now;
        feedback.Id = ObjectId.GenerateNewId().ToString();
        await unitOfWork.FeedbackRepository.AddAsync(feedback);
        await unitOfWork.SaveChangesAsync();

        session.FeedbackId = feedback.Id;
        await unitOfWork.ExchangeSessionRepository.UpdateAsync(session.Id, session);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<FeedbackResponeDto>> GetAllFeedbackOfProductSaleAsync(string sellProductId)
    {
        return await unitOfWork.FeedbackRepository.GetAllFeedbackOfProductSaleAsync(sellProductId);
    }
}