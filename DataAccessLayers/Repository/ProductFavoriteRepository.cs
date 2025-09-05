using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
using BusinessObjects.Mongodb;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class ProductFavoriteRepository(MongoDbContext context, IOptions<FavoritesSettings> favoritesSettings)
    : GenericRepository<ProductFavorite>(context.GetCollection<ProductFavorite>("ProductFavorite")),
        IProductFavoriteRepository
{
    private readonly FavoritesSettings _favoriteSettings = favoritesSettings.Value;
    private readonly IMongoCollection<Product> _productCollection = context.GetCollection<Product>("Product");
    private readonly IMongoCollection<ProductFavorite> _productFavorite = context.GetCollection<ProductFavorite>("ProductFavorite");
    private readonly IMongoCollection<Rarity> _rarityCollection = context.GetCollection<Rarity>("Rarity");
    private readonly IMongoCollection<UserProduct> _userProductCollection = context.GetCollection<UserProduct>("User_Product");

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
            new()
            {
                Id = _favoriteSettings.CollectionTopic,
                UserId = userId,
                CollectionId = null,
                CollectionTopic = _favoriteSettings.CollectionTopic,
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
        var userProductIds = productFavorites.Select(x => x.User_productId).Distinct().ToList();
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


        var result = await Task.WhenAll(productFavorites.Select(async p =>
            {
                var userProduct = userProducts.FirstOrDefault(x => x.Id.Equals(p.User_productId));
                var productId = userProduct.ProductId;
                var hasProduct = productDict.TryGetValue(productId, out var product);
                var rarity = await _rarityCollection.Find(x => x.Id == userProduct.ProductId).FirstOrDefaultAsync();
                var rarityName = rarity?.Name ?? "Unknown";

                return new CollectionProductsDto
                {
                    Id = p.Id.ToString(),
                    CollectionId = userProduct.CollectionId,
                    ProductId = productId,
                    Quantity = userProduct.Quantity,
                    CollectedAt = userProduct.CollectedAt,
                    CollectorId = userProduct.CollectorId,
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