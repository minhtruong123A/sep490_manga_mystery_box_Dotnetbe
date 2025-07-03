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
            var userCollections = await _userCollection.Find(c => c.UserId == userId).ToListAsync();
            if (!userCollections.Any()) return [];

            var collectionIds = userCollections.Select(c => c.CollectionId).ToList();
            var collectionsTask = _collection.Find(c => collectionIds.Contains(c.Id.ToString())).ToListAsync();
            var userProductsTask = _userProductCollection.Find(c => c.CollectorId == userId && collectionIds.Contains(c.CollectionId)).ToListAsync();
            await Task.WhenAll(collectionsTask, userProductsTask);

            var collections = collectionsTask.Result.ToDictionary(c => c.Id.ToString());
            var userProducts = userProductsTask.Result;
            var productIds = userProducts.Select(p => p.ProductId).Distinct().ToList();
            var products = (await _productCollection.Find(p => productIds.Contains(p.Id.ToString())).ToListAsync()).ToDictionary(p => p.Id.ToString());

            return userCollections.Select(col =>
            {
                var topic = collections.GetValueOrDefault(col.CollectionId)?.Topic ?? "No topic";
                var images = userProducts
                    .Where(p => p.CollectionId == col.CollectionId)
                    .Select(p => products.TryGetValue(p.ProductId, out var prod)
                        ? new Collection_sProductsImageDto { Id = p.ProductId, UrlImage = prod.UrlImage }
                        : null)
                    .Where(p => p != null).ToList();

                return new UserCollectionGetAllDto
                {
                    Id = col.Id,
                    UserId = col.UserId,
                    CollectionId = col.CollectionId,
                    CollectionTopic = topic,
                    Image = images,
                    Count = images.Count
                };
            }).ToList();
        }

    }
}
