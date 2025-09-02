using BusinessObjects;
using BusinessObjects.Dtos.Exchange;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Report;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccessLayers.Repository
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        private readonly IMongoCollection<Report> _reportCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<SellProduct> _sellProductCollection;
        private readonly IMongoCollection<User> _userCollection;
        public ReportRepository(MongoDbContext context) : base(context.GetCollection<Report>("Report"))
        {
            _reportCollection = context.GetCollection<Report>("Report");
            _productCollection = context.GetCollection<Product>("Product");
            _sellProductCollection = context.GetCollection<SellProduct>("SellProduct");
            _userCollection = context.GetCollection<User>("User");
        }

        public async Task<bool> CreateReportAsync(ReportCreateDto dto, string userId)
        {
            if (userId.Equals(dto.SellerId)) throw new Exception("Hệ thống không chấp nhận sự ngu ngốc này!");
            if (dto.Title == null || dto.Content == null) throw new Exception("Please fill title or content");
            var newReport = new Report
            {
                UserId = userId,
                SellerId = dto.SellerId,
                SellProductId = dto.SellProductId,
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.Now,
                Status = false
            };
            await _reportCollection.InsertOneAsync(newReport);
            return true;
        }

        public async Task<List<ReportResponeDto>> GetAllReportOfUserAsync(string userId)
        {
            var reports = await _reportCollection.Find(x => x.UserId.Equals(userId)).ToListAsync();
            if (!reports.Any()) return [];

            var sellProductIds = reports.Select(x => x.SellProductId).Distinct().ToList();
            var sellerIds = reports.Select(x => x.SellerId).Distinct().ToList();

            var validSellerIdStrings = sellerIds
                .Where(id => ObjectId.TryParse(id, out _))
                .ToList();

            var validSellProductIdStrings = sellProductIds
            .Where(id => ObjectId.TryParse(id, out _))
            .ToList();

            var sellProducts = await _sellProductCollection
                .Find(x => validSellProductIdStrings.Contains(x.Id))
                .ToListAsync();

            var productIds = sellProducts.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productCollection.Find(x => productIds.Contains(x.Id)).ToListAsync();
            var users = await _userCollection.Find(x => validSellerIdStrings.Contains(x.Id)).ToListAsync();

            return reports.Select(report =>
            {
                var sellproduct = sellProducts.FirstOrDefault(x => x.Id == report.SellProductId);
                var product = products.FirstOrDefault(x => x.Id == sellproduct?.ProductId);
                var user = users.FirstOrDefault(x => x.Id == report.SellerId);

                return new ReportResponeDto
                {
                    Id = report.Id,
                    SellerId = report.SellerId,
                    Content = report.Content,
                    CreatedAt = report.CreatedAt, // tốt hơn dùng thời gian tạo từ DB thay vì DateTime.Now
                    ProductName = product?.Name ?? "Unknown",
                    SellerName = user?.Username ?? "Unknown",
                    SellProductId = sellproduct?.Id ?? "Unknown",
                    Status = report.Status,
                    Title = report.Title,
                    UserId = report.UserId
                };
            }).ToList();
        }
        public async Task<List<ReportResponeDto>> GetAllReportAsync()
        {
            var reports = await _reportCollection.AsQueryable().ToListAsync();
            if (!reports.Any()) return [];

            var sellProductIds = reports.Select(x => x.SellProductId).Distinct().ToList();
            var sellerIds = reports.Select(x => x.SellerId).Distinct().ToList();
            var userReportIds = reports.Select(x => x.UserId).Distinct().ToList();

            var validSellerIdStrings = sellerIds
                .Where(id => ObjectId.TryParse(id, out _))
                .ToList();
            var validUserIdStrings = userReportIds
                .Where(id => ObjectId.TryParse(id, out _))
                .ToList();
            var validSellProductIdStrings = sellProductIds
            .Where(id => ObjectId.TryParse(id, out _))
            .ToList();

            var sellProducts = await _sellProductCollection
                .Find(x => validSellProductIdStrings.Contains(x.Id))
                .ToListAsync();

            var productIds = sellProducts.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productCollection.Find(x => productIds.Contains(x.Id)).ToListAsync();
            var users = await _userCollection.Find(x => validSellerIdStrings.Contains(x.Id)).ToListAsync();
            var userReports = await _userCollection.Find(x => validUserIdStrings.Contains(x.Id)).ToListAsync();

            return reports.Select(report =>
            {
                var sellproduct = sellProducts.FirstOrDefault(x => x.Id == report.SellProductId);
                var product = products.FirstOrDefault(x => x.Id == sellproduct?.ProductId);
                var user = users.FirstOrDefault(x => x.Id == report.SellerId);
                var userReport = userReports.FirstOrDefault(x => x.Id.Equals(report.UserId));
                return new ReportResponeDto
                {
                    Id = report.Id,
                    SellerId = report.SellerId,
                    Content = report.Content,
                    CreatedAt = report.CreatedAt, 
                    ProductName = product?.Name,
                    SellerName = user?.Username ,
                    SellProductId = sellproduct?.Id ,
                    Status = report.Status,
                    Title = report.Title,
                    UserId = report.UserId,
                    UserName = userReport?.Username ,
                };
            }).ToList();
        }

    }
}
