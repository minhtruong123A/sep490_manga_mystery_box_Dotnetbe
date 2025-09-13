using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class UserCollectionRepository(MongoDbContext context, IUserAchievementRepository userAchievementRepository)
    : GenericRepository<UserCollection>(context.GetCollection<UserCollection>("UserCollection")),
        IUserCollectionRepository
{
    private readonly IMongoCollection<Collection> _collection = context.GetCollection<Collection>("Collection");
    private readonly IMongoCollection<Product> _productCollection = context.GetCollection<Product>("Product");
    private readonly IMongoCollection<UserCollection> _userCollection = context.GetCollection<UserCollection>("UserCollection");
    private readonly IMongoCollection<UserProduct> _userProductCollection = context.GetCollection<UserProduct>("User_Product");

    public async Task<List<UserCollectionGetAllDto>> GetAllWithDetailsAsync(string userId)
    {
        await userAchievementRepository.CheckAchievement(userId);
        // 1. Lấy danh sách UserCollection của user
        var userCollections = await _userCollection.Find(c => c.UserId == userId).ToListAsync();
        if (!userCollections.Any()) return [];

        var userCollectionIds = userCollections.Select(c => c.Id).ToList();

        // 2. Lấy danh sách Collection để lấy Topic
        var collectionIds = userCollections.Select(c => c.CollectionId).Distinct().ToList();
        var collections = await _collection
            .Find(c => collectionIds.Contains(c.Id.ToString()))
            .ToListAsync();
        var collectionDict = collections.ToDictionary(c => c.Id.ToString());

        // 3. Lấy toàn bộ UserProduct (KHÔNG lọc Quantity)
        var userProductsAll = await _userProductCollection
            .Find(up => userCollectionIds.Contains(up.CollectionId))
            .ToListAsync();

        // 3.1 Lọc UserProduct có Quantity > 0 để dùng cho Image
        var userProductsAvailable = userProductsAll
            .Where(up => up.Quantity > 0)
            .ToList();

        // 4. Lấy danh sách Product theo ProductId
        var productIds = userProductsAll.Select(up => up.ProductId).Distinct().ToList();
        var products = await _productCollection
            .Find(p => productIds.Contains(p.Id.ToString()))
            .ToListAsync();
        var productDict = products.ToDictionary(p => p.Id.ToString());

        // 5. Mapping
        var result = userCollections.Select(uc =>
        {
            var topic = collectionDict.GetValueOrDefault(uc.CollectionId)?.Topic ?? "No topic";
            var images = userProductsAvailable
                .Where(up => up.CollectionId == uc.Id)
                .Select(up => productDict.TryGetValue(up.ProductId, out var prod)
                    ? new Collection_sProductsImageDto
                    {
                        Id = up.ProductId,
                        UrlImage = prod.UrlImage
                    }
                    : null)
                .Where(img => img != null)
                .Take(4)
                .ToList();
            var count = userProductsAll
                .Where(up => up.CollectionId == uc.Id)
                .Select(up => productDict.TryGetValue(up.ProductId, out var prod)
                    ? new Collection_sProductsImageDto
                    {
                        Id = up.ProductId,
                        UrlImage = prod.UrlImage
                    }
                    : null)
                .Where(img => img != null)
                .ToList().Count();
            return new UserCollectionGetAllDto
            {
                Id = uc.Id,
                UserId = uc.UserId,
                CollectionId = uc.CollectionId,
                CollectionTopic = topic,
                Image = images,
                Count = count
            };
        }).ToList();

        return result;
    }
}