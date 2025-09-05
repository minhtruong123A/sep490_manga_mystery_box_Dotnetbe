using BusinessObjects;
using BusinessObjects.Dtos.Comment;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class CommentRepository(MongoDbContext context)
    : GenericRepository<Comment>(context.GetCollection<Comment>("Comment")), ICommentRepository
{
    private readonly IMongoCollection<Comment> _comments = context.GetCollection<Comment>("Comment");
    private readonly IMongoCollection<Product> _product = context.GetCollection<Product>("Product");
    private readonly IMongoCollection<SellProduct> _sellProduct = context.GetCollection<SellProduct>("SellProduct");
    private readonly IMongoCollection<User> _users = context.GetCollection<User>("User");

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
        var userDict = users.ToDictionary(u => u.Id, u => new { u.Username, u.ProfileImage });
        var result = comments.Select(c => new CommentWithUsernameDto
        {
            Id = c.Id,
            SellProductId = c.SellProductId,
            Username = userDict.ContainsKey(c.UserId) ? userDict[c.UserId].Username : "Unknown",
            ProfileImage = userDict.ContainsKey(c.UserId) ? userDict[c.UserId].ProfileImage : null,
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
        var userDict = users.ToDictionary(u => u.Id, u => new { u.Username, u.ProfileImage });
        var result = comments.Select(c => new RatingWithUsernameDto
        {
            Id = c.Id,
            SellProductId = c.SellProductId,
            Username = userDict.ContainsKey(c.UserId) ? userDict[c.UserId].Username : "Unknown",
            ProfileImage = userDict.ContainsKey(c.UserId) ? userDict[c.UserId].ProfileImage : null,
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

    public async Task<Comment?> GetCommentOnlyByUserAndProductAsync(string userId, string productId)
    {
        return await _comments.Find(c =>
            c.UserId == userId &&
            c.SellProductId == productId &&
            c.Rating == -1
        ).SingleOrDefaultAsync();
    }

    public async Task<float> GetRatingOfUserAsync(string userId)
    {
        var existUser = await _users.Find(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
        if (existUser == null) throw new Exception("User not exist");
        var sellProducts = await _sellProduct.Find(x => x.SellerId.Equals(userId)).ToListAsync();
        if (!sellProducts.Any()) throw new Exception("User don't have any sell product");
        var sellProductIds = sellProducts.Select(x => x.Id).Distinct().ToList();
        var comments = await _comments.Find(c => sellProductIds.Contains(c.SellProductId) && c.Rating != -1)
            .ToListAsync();
        float total = 0;

        if (comments.Any())
        {
            foreach (var comment in comments) total += comment.Rating;
            return total / comments.Count;
        }

        return total;
    }

    public async Task<float> GetTotalAverageOfSellProductByIdAsync(string sellProductId)
    {
        var comments = await _comments.Find(c => c.SellProductId.Equals(sellProductId) && c.Rating != -1).ToListAsync();
        float total = 0;

        if (comments.Any())
        {
            foreach (var comment in comments) total += comment.Rating;
            return total / comments.Count;
        }

        return total;
    }

    public async Task<List<CommentWithUsernameDto>> GetAllCommentProductOfUserAsync(string userId, string productName)
    {
        var product = await _product.Find(x => x.Name.Equals(productName)).FirstOrDefaultAsync();
        var sellProducts = await _sellProduct.Find(x => x.SellerId.Equals(userId) &&
                                                        x.ProductId == product.Id).ToListAsync();
        var sellProductIds = sellProducts.Select(x => x.Id).Distinct().ToList();
        var comments = await _comments.Find(c => sellProductIds.Contains(c.SellProductId) && c.Rating == -1)
            .SortByDescending(c => c.CreatedAt)
            .ToListAsync();
        var userIds = comments.Select(c => c.UserId).Distinct().ToList();
        var users = await _users.Find(u => userIds.Contains(u.Id)).ToListAsync();
        var userDict = users.ToDictionary(u => u.Id, u => new { u.Username, u.ProfileImage });
        var result = comments.Select(c => new CommentWithUsernameDto
        {
            Id = c.Id,
            SellProductId = c.SellProductId,
            Username = userDict.ContainsKey(c.UserId) ? userDict[c.UserId].Username : "Unknown",
            ProfileImage = userDict.ContainsKey(c.UserId) ? userDict[c.UserId].ProfileImage : null,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            Status = c.Status
        }).ToList();

        return result;
    }
}