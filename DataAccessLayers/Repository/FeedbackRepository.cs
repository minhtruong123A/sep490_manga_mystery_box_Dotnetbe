using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        private readonly IMongoCollection<ExchangeInfo> _exchangeInfo;
        private readonly IMongoCollection<ExchangeProduct> _exchangeProduct;
        private readonly IMongoCollection<ExchangeSession> _exchangeSession;
        private readonly IMongoCollection<Feedback> _feedback;
        public FeedbackRepository(MongoDbContext context) : base(context.GetCollection<Feedback>("Feedback"))
        {
            _feedback = context.GetCollection<Feedback>("Feedback");
            _exchangeInfo = context.GetCollection<ExchangeInfo>("Exchangeinfo");
            _exchangeProduct = context.GetCollection<ExchangeProduct>("ExchangeProduct");
            _exchangeSession = context.GetCollection<ExchangeSession>("ExchangeSession");
        }

        public async Task<List<Feedback>> GetAllFeedbackOfProductSaleAsync (string sellProductId)
        {
            var exchange_infos = await _exchangeInfo.Find(x => x.ItemReciveId.Equals(sellProductId)).ToListAsync();

            var itemGiveIds = exchange_infos.Select(x => x.ItemGiveId).Distinct().ToList();
            var sessions = await _exchangeSession.Find(p => itemGiveIds.Contains(p.Id)).ToListAsync();

            var feedbackIds = sessions.Select(x => x.FeedbackId).Distinct().ToList();
            var feedbacks = await _feedback.Find(p => feedbackIds.Contains(p.Id)).ToListAsync();

            return feedbacks;
        }
    }
}
