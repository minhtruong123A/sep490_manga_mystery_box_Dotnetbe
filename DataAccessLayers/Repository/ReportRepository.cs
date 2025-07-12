using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Report;
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
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        private readonly IMongoCollection<Report> _reportCollection;
        public ReportRepository(MongoDbContext context) : base(context.GetCollection<Report>("Report"))
        {
            _reportCollection = context.GetCollection<Report>("Report");
        }

        public async Task<bool> CreateReportAsync(ReportCreateDto dto, string userId)
        {
            if (userId.Equals(dto.SellerId)) throw new Exception("Hệ thống không chấp nhận sự ngu ngốc này!");
            var newReport = new Report
            {
                UserId = userId,
                SellerId = dto.SellerId,
                SellProductId = dto.SellProductId,
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.Now
            };
            await _reportCollection.InsertOneAsync(newReport);
            return true;
        }

        public async Task<List<Report>> GetAllReportOfUserAsync(string userId)
        {
           return await _reportCollection.Find(x=> x.UserId.Equals(userId)).ToListAsync();
        }
    }
}
