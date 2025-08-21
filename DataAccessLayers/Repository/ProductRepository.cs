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
        private readonly IMongoCollection<SellProduct> _sellProductCollection;
        private readonly IMongoCollection<ProductInMangaBox> _productInMangaBoxCollection;
        private readonly IMongoCollection<MangaBox> _mangaBoxCollection;
        private readonly ISellProductRepository _sellProductRepository;

        public ProductRepository(MongoDbContext context, ISellProductRepository sellProductRepository) : base(context.GetCollection<Product>("Product"))
        {
            _productCollection = context.GetCollection<Product>("Product");
            _rarityCollection = context.GetCollection<Rarity>("Rarity");
            _productInMangaBoxCollection = context.GetCollection<ProductInMangaBox>("ProductInMangaBox");
            _sellProductCollection = context.GetCollection<SellProduct>("SellProduct");
            _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
            _sellProductRepository = sellProductRepository;
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

        public async Task CheckProduct(string productId)
        {
            var productInBoxs = await _productInMangaBoxCollection.Find(x => x.ProductId.Equals(productId)).ToListAsync();
            var product = await _productCollection.Find(x=>x.Id.Equals(productId)).FirstOrDefaultAsync();
            var boxIds = productInBoxs.Select(x=>x.MangaBoxId).Distinct().ToList();
            var mangaBoxs = await _mangaBoxCollection.Find(x => boxIds.Contains(x.Id)).ToListAsync();
            foreach (var box in mangaBoxs)
            {
                if (product.Is_Block)
                {
                    var updateStatus = Builders<MangaBox>.Update.Set(x => x.Status, 0);
                    await _mangaBoxCollection.UpdateOneAsync(box.Id, updateStatus);
                }
                else
                {
                    var productBoxs = await _productInMangaBoxCollection.Find(x=>x.MangaBoxId.Equals(box.Id)).ToListAsync();
                    var productBoxIds = productBoxs.Select(x=>x.ProductId).Distinct().ToList();
                    var productIsBlocks = await _productCollection.Find(x=> productBoxIds.Contains(x.Id) && x.Is_Block==true).ToListAsync();
                    if (!productIsBlocks.Any())
                    {
                        var updateStatus = Builders<MangaBox>.Update.Set(x => x.Status, 1);
                        await _mangaBoxCollection.UpdateOneAsync(box.Id, updateStatus);
                    }
                }
            }
            if (product.Is_Block)
            {
                var sellProducts = await _sellProductCollection.Find(x => x.ProductId.Equals(productId) && x.IsSell == true).ToListAsync();
                foreach (var sellProduct in sellProducts)
                {
                    await _sellProductRepository.CancelSellProductAsync(sellProduct.Id);
                }
            }
        }
    }
}
