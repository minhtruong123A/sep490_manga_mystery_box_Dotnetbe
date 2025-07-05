using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class UserCollectionRepository : GenericRepository<UserCollection>, IUserCollectionRepository
    {
        private readonly IMongoCollection<UserCollection> _userCollection;
        private readonly IMongoCollection<Collection> _collection;
        private readonly IMongoCollection<UserProduct> _userProductCollection;
        private readonly IMongoCollection<Product> _productCollection;
        public UserCollectionRepository(MongoDbContext context) : base(context.GetCollection<UserCollection>("UserCollection"))
        {
            _userCollection = context.GetCollection<UserCollection>("UserCollection");
            _collection = context.GetCollection<Collection>("Collection");
            _userProductCollection = context.GetCollection<UserProduct>("User_Product");
            _productCollection = context.GetCollection<Product>("Product");
        }

        public async Task<List<UserCollectionGetAllDto>> GetAllWithDetailsAsync(string userId)
        {
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

            // 3. Lấy danh sách UserProduct theo các UserCollection.Id
            var userProducts = await _userProductCollection
                .Find(up => userCollectionIds.Contains(up.CollectionId))
                .ToListAsync();

            // 4. Lấy danh sách Product theo ProductId
            var productIds = userProducts.Select(up => up.ProductId).Distinct().ToList();
            var products = await _productCollection
                .Find(p => productIds.Contains(p.Id.ToString()))
                .ToListAsync();
            var productDict = products.ToDictionary(p => p.Id.ToString());

            // 5. Mapping
            var result = userCollections.Select(uc =>
            {
                var topic = collectionDict.GetValueOrDefault(uc.CollectionId)?.Topic ?? "No topic";
                var images = userProducts
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

                return new UserCollectionGetAllDto
                {
                    Id = uc.Id,
                    UserId = uc.UserId,
                    CollectionId = uc.CollectionId,
                    CollectionTopic = topic,
                    Image = images,
                    Count = images.Count
                };
            }).ToList();

            return result;
        }

    }
}
