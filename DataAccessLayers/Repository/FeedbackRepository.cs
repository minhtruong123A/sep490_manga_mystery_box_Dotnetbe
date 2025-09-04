using BusinessObjects;
using BusinessObjects.Dtos.Feedback;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class FeedbackRepository(MongoDbContext context)
    : GenericRepository<Feedback>(context.GetCollection<Feedback>("Feedback")), IFeedbackRepository
{
    private readonly IMongoCollection<ExchangeInfo> _exchangeInfo = context.GetCollection<ExchangeInfo>("Exchangeinfo");
    private readonly IMongoCollection<ExchangeProduct> _exchangeProduct = context.GetCollection<ExchangeProduct>("ExchangeProduct");
    private readonly IMongoCollection<ExchangeSession> _exchangeSession = context.GetCollection<ExchangeSession>("ExchangeSession");
    private readonly IMongoCollection<Feedback> _feedback = context.GetCollection<Feedback>("Feedback");
    private readonly IMongoCollection<User> _user = context.GetCollection<User>("User");

    public async Task<List<FeedbackResponeDto>> GetAllFeedbackOfProductSaleAsync(string sellProductId)
    {
        var exchange_infos = await _exchangeInfo.Find(x => x.ItemReciveId.Equals(sellProductId)).ToListAsync();

        var itemGiveIds = exchange_infos.Select(x => x.ItemGiveId).Distinct().ToList();
        var sessions = await _exchangeSession.Find(p => itemGiveIds.Contains(p.Id)).ToListAsync();

        var feedbackIds = sessions.Select(x => x.FeedbackId).Distinct().ToList();
        var feedbacks = await _feedback.Find(p => feedbackIds.Contains(p.Id)).ToListAsync();
        var userIds = feedbacks.Select(x => x.UserId).Distinct().ToList();
        var users = await _user.Find(x => userIds.Contains(x.Id)).ToListAsync();

        return feedbacks.Select(feedback =>
        {
            var user = users.FirstOrDefault(x => x.Id.Equals(feedback.UserId));
            return new FeedbackResponeDto
            {
                Id = feedback.Id,
                Content = feedback.Content,
                Rating = feedback.Rating,
                UserName = user.Username,
                CreateAt = feedback.CreateAt
            };
        }).ToList();
    }

    public async Task<bool> CheckExistFeedbackAsync(string userId)
    {
        var exist = await _feedback.Find(x => x.UserId.Equals(userId)).FirstOrDefaultAsync();
        if (exist != null) return false;
        return true;
    }
}