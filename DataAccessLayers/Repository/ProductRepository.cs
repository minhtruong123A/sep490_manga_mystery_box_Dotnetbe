using BusinessObjects;
using BusinessObjects.Dtos.Product;
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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Rarity> _rarityCollection;

        public ProductRepository(MongoDbContext context) : base(context.GetCollection<Product>("Product"))
        {
            _productCollection = context.GetCollection<Product>("Product");
            _rarityCollection = context.GetCollection<Rarity>("Rarity");
        }

        public async Task<ProductWithRarityDto?> GetProductWithRarityByIdAsync(string productId)
        {
            var product = await _productCollection.AsQueryable().FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return null;

            var rarity = product.RarityId != null 
                ? await _rarityCollection.AsQueryable().FirstOrDefaultAsync(r => r.Id == product.RarityId)
                : null;

            return new ProductWithRarityDto
            {
                ProductId = product.Id,
                Name = product.Name,
                UrlImage = product.UrlImage,
                Description = product.Description,
                RarityName = rarity?.Name ?? "Unknown",
            };
        }

        public async Task<List<ProductWithRarityForModeratorDto>> GetAllProductsWithRarityAsync()
        {
            var products = await _productCollection.Find(_ => true).ToListAsync();

            var tasks = products.Select(async product =>
            {
                var rarity = await _rarityCollection.Find(r => r.Id == product.RarityId).FirstOrDefaultAsync();
                return new ProductWithRarityForModeratorDto
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    UrlImage = product.UrlImage,
                    Description = product.Description,
                    RarityName = rarity?.Name ?? "Unknown",   
                    CollectionId = product.CollectionId,
                    is_Block = product.Is_Block
                };
            });

            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }
    }
}
