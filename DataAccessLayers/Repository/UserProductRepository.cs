using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserBox;
using BusinessObjects.Dtos.UserCollection;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class UserProductRepository : GenericRepository<UserProduct>, IUserProductRepository
    {

        private readonly IMongoCollection<UserProduct> _userProductCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Rarity> _rarityCollection;

        public UserProductRepository(MongoDbContext context) : base(context.GetCollection<UserProduct>("User_Product"))
        {
            _userProductCollection = context.GetCollection<UserProduct>("User_Product");
            _productCollection = context.GetCollection<Product>("Product");
            _rarityCollection = context.GetCollection<Rarity>("Rarity");
        }
        public async Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId, string userCollectionId)
        {
            
            var userProducts = await _userProductCollection
                .Find(p => p.CollectorId == userId && p.CollectionId == userCollectionId)
                .ToListAsync();

            if (!userProducts.Any())
                return new List<CollectionProductsDto>();

           
            var productIds = userProducts.Select(p => p.ProductId.Trim()).Distinct().ToList();

            
            var products = await _productCollection
                .Find(p => productIds.Contains(p.Id.ToString()))
                .ToListAsync();

            var productDict = products.ToDictionary(p => p.Id.ToString());


            var result = await Task.WhenAll(userProducts.Select(async p =>
            {
                var productId = p.ProductId.Trim();
                var hasProduct = productDict.TryGetValue(productId, out var product);
                var rarity = await _rarityCollection.Find(x => x.Id == product.RarityId).FirstOrDefaultAsync();
                var rarityName = rarity?.Name ?? "Unknown";

                return new CollectionProductsDto
                {
                    Id = p.Id.ToString(),
                    CollectionId = userCollectionId,
                    ProductId = productId,
                    Quantity = p.Quantity,
                    CollectedAt = p.CollectedAt,
                    CollectorId = p.CollectorId,
                    ProductName = hasProduct ? product.Name : null,
                    UrlImage = hasProduct ? product.UrlImage : null,
                    RarityName = rarityName,
                    UpdateAt = p.UpdateAt,
                    isQuantityUpdateInc = p.isQuantityUpdateInc,
                    Product_isBlock = product.Is_Block,
                };
            }));

            return result.ToList();

        }
        public async Task AddManyAsync(IEnumerable<UserProduct> userProducts)
        {
            await _userProductCollection.InsertManyAsync(userProducts);
        }
        public async Task<UserProduct> FindOneAsync(Expression<Func<UserProduct, bool>> filter)
        {
            return await _userProductCollection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task UpdateOneAsync(string id, UpdateDefinition<UserProduct> update)
        {
            var filter = Builders<UserProduct>.Filter.Eq(up => up.Id, id);
            await _userProductCollection.UpdateOneAsync(filter, update);
        }
        public async Task AddAsync(UserProduct userProduct)
        {
            if (userProduct == null) return;
            await _userProductCollection.InsertOneAsync(userProduct);
        }

    }
}
