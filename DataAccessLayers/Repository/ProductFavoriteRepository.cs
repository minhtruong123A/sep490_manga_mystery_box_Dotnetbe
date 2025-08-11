using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
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
    public class ProductFavoriteRepository : GenericRepository<ProductFavorite>, IProductFavoriteRepository
    {
        private readonly IMongoCollection<ProductFavorite> _productFavorite;
        private readonly IMongoCollection<UserProduct> _userProductCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Rarity> _rarityCollection;

        public ProductFavoriteRepository(MongoDbContext context) : base(context.GetCollection<ProductFavorite>("ProductFavorite"))
        {
            _productFavorite = context.GetCollection<ProductFavorite>("ProductFavorite");
            _userProductCollection = context.GetCollection<UserProduct>("User_Product");
            _productCollection = context.GetCollection<Product>("Product");
            _rarityCollection = context.GetCollection<Rarity>("Rarity");
        }

        public async Task<List<UserCollectionGetAllDto>> GetFavoriteListWithDetailsAsync(string userId)
        {
            var productFavorites = await _productFavorite
                .Find(p => p.User_Id == userId)
                .ToListAsync();
            if (!productFavorites.Any()) return [];

            var userProductIds = productFavorites
                .Select(pf => pf.User_productId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();
            var userProductsAll = await _userProductCollection
                .Find(up => userProductIds.Contains(up.Id))
                .ToListAsync();
            if (!userProductsAll.Any()) return [];

            var userProductsAvailable = userProductsAll
                .Where(up => up.Quantity > 0)
                .ToList();
            var productIds = userProductsAll
                .Select(up => up.ProductId.Trim())
                .Distinct()
                .ToList();
            var products = await _productCollection
                .Find(p => productIds.Contains(p.Id.ToString()))
                .ToListAsync();
            var productDict = products.ToDictionary(p => p.Id.ToString());
            var images = userProductsAvailable
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

            return new List<UserCollectionGetAllDto>
            {
                new UserCollectionGetAllDto
                {
                    Id = "FavoriteList",
                    UserId = userId,
                    CollectionId = null,
                    CollectionTopic = "Favorite List",
                    Image = images,
                    Count = 0
                }
            };
        }

        public async Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId)
        {
            await CheckUserProductValid(userId);
            var productFavorites = await _productFavorite
                .Find(p => p.User_Id == userId)
                .ToListAsync();
            var userProductIds = productFavorites.Select(x=>x.User_productId).Distinct().ToList();
            var userProducts = await _userProductCollection
                .Find(p => userProductIds.Contains(p.Id.ToString()) && p.CollectorId.Equals(userId))
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
                var rarity = await _rarityCollection.Find(x => x.Id == p.ProductId).FirstOrDefaultAsync();
                var rarityName = rarity?.Name ?? "Unknown";

                return new CollectionProductsDto
                {
                    Id = p.Id.ToString(),
                    CollectionId = p.CollectionId,
                    ProductId = productId,
                    Quantity = p.Quantity,
                    CollectedAt = p.CollectedAt,
                    CollectorId = p.CollectorId,
                    ProductName = hasProduct ? product.Name : null,
                    UrlImage = hasProduct ? product.UrlImage : null,
                    RarityName = rarityName
                };
            }).ToList()
            );

            return result.ToList();

        }

        public async Task CheckUserProductValid(string userId)
        {
            var productFavorites = await _productFavorite
                .Find(p => p.User_Id == userId)
                .ToListAsync();
            var userProductIds = productFavorites.Select(x => x.User_productId).Distinct().ToList();
            var userProducts = await _userProductCollection
                .Find(p => userProductIds.Contains(p.Id))
                .ToListAsync();
            var userProductNotValid = userProducts.Where(x => x.Quantity == 0).ToList();
            if (!userProductNotValid.Any()) return;
            foreach (var userProduct in userProductNotValid)
            {
                var filter = Builders<ProductFavorite>.Filter.Where(x => x.User_productId.Equals(userProduct.Id));
                await _productFavorite.DeleteOneAsync(filter);
            }
        }
    }
}
