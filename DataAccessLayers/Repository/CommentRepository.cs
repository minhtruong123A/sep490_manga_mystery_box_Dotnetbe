using BusinessObjects.Mongodb;
using BusinessObjects;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayers.Interface;
using BusinessObjects.Dtos.Comment;

namespace DataAccessLayers.Repository
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly IMongoCollection<Comment> _comments;
        private readonly IMongoCollection<User> _users;

        public CommentRepository(MongoDbContext context) : base(context.GetCollection<Comment>("Comment"))
        {
            _comments = context.GetCollection<Comment>("Comment");
            _users = context.GetCollection<User>("User");
        }

        public async Task<Comment> CreateCommentAsync(string sellProductId, string userId, string content)
        {
            var comment = new Comment
            {
                Id = ObjectId.GenerateNewId().ToString(),
                SellProductId = sellProductId,
                UserId = userId,
                Content = content,
                Rating = -1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = 1
            };

            await _comments.InsertOneAsync(comment);
            return comment;
        }

        public async Task<Comment> CreateRatingOnlyAsync(string sellProductId, string userId, float rating)
        {
            var comment = new Comment
            {
                Id = ObjectId.GenerateNewId().ToString(),
                SellProductId = sellProductId,
                UserId = userId,
                Content = string.Empty,
                Rating = rating,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = 1
            };

            await _comments.InsertOneAsync(comment);
            return comment;
        }


        public async Task<List<CommentWithUsernameDto>> GetAlCommentlBySellProductIdAsync(string sellProductId)
        {
            var comments = await _comments.Find(c => c.SellProductId == sellProductId
                                                   && c.Status == 1
                                                   && c.Rating == -1)
                                          .SortByDescending(c => c.CreatedAt)
                                          .ToListAsync();
            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            var users = await _users.Find(u => userIds.Contains(u.Id)).ToListAsync();
            var userDict = users.ToDictionary(u => u.Id, u => u.Username);
            var result = comments.Select(c => new CommentWithUsernameDto
            {
                Id = c.Id,
                SellProductId = c.SellProductId,
                Username = userDict.ContainsKey(c.UserId) ? userDict[c.UserId] : "Unknown",
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Status = c.Status
            }).ToList();

            return result;
        }

        public async Task<List<RatingWithUsernameDto>> GetAllRatingBySellProductIdAsync(string sellProductId)
        {
            var comments = await _comments.Find(c => c.SellProductId == sellProductId
                                                   && c.Status == 1
                                                   && c.Content == string.Empty)
                                          .SortByDescending(c => c.CreatedAt)
                                          .ToListAsync();
            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            var users = await _users.Find(u => userIds.Contains(u.Id)).ToListAsync();
            var userDict = users.ToDictionary(u => u.Id, u => u.Username);
            var result = comments.Select(c => new RatingWithUsernameDto
            {
                Id = c.Id,
                SellProductId = c.SellProductId,
                Username = userDict.ContainsKey(c.UserId) ? userDict[c.UserId] : "Unknown",
                Rating = c.Rating,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Status = c.Status
            }).ToList();

            return result;
        }

        public async Task<Comment?> GetRatingOnlyByUserAndProductAsync(string userId, string productId)
        {
            return await _comments.Find(c =>
                c.UserId == userId &&
                c.SellProductId == productId &&
                (c.Content == null || c.Content.Trim() == "")
            ).SingleOrDefaultAsync();
        }
    }
}