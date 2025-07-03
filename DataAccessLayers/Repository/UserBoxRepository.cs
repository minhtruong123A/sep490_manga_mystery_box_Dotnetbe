using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserBox;
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
    public class UserBoxRepository : GenericRepository<UserBox>, IUserBoxRepository
    {
        private readonly IMongoCollection<UserBox> _userBoxCollection;
        private readonly IMongoCollection<MangaBox> _mangaBoxCollection;
        private readonly IMongoCollection<UserCollection> _userCollectionCollection;
        private readonly IMongoCollection<UserProduct> _userProductCollection;
        private readonly IMongoCollection<Collection> _collectionCollection;

        private readonly IMangaBoxRepository _mangaBoxRepository;
        public UserBoxRepository(MongoDbContext context, IMangaBoxRepository mangaBoxRepository) : base(context.GetCollection<UserBox>("UserBox"))
        {
            _userBoxCollection = context.GetCollection<UserBox>("UserBox");
            _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
            _userCollectionCollection = context.GetCollection<UserCollection>("UserCollection");
            _userProductCollection = context.GetCollection<UserProduct>("User_Product");
            _collectionCollection = context.GetCollection<Collection>("Collection");
            _mangaBoxRepository = mangaBoxRepository;
        }

        public async Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId)
        {
            // 1. Lấy các UserBox của user
            var userBox = await _userBoxCollection.Find(c => c.UserId == userId).ToListAsync();
            if (!userBox.Any()) return [];

            // 2. Lấy các MangaBox tương ứng
            var boxIds = userBox.Select(c => c.BoxId).ToList();
            var mangaBoxes = await _mangaBoxCollection.Find(c => boxIds.Contains(c.Id.ToString())).ToListAsync();
            var mangaBoxDict = mangaBoxes.ToDictionary(c => c.Id.ToString());

            // 3. Lấy các Collection tương ứng (dựa vào CollectionTopicId của MangaBox)
            var collectionIds = mangaBoxes.Select(c => c.CollectionTopicId).Distinct().ToList();
            var collections = await _collectionCollection.Find(c => collectionIds.Contains(c.Id.ToString())).ToListAsync();
            var collectionDict = collections.ToDictionary(c => c.Id.ToString());

            // 4. Mapping sang DTO
            return userBox.Select(box =>
            {
                var matchedMangaBox = mangaBoxDict.GetValueOrDefault(box.BoxId);
                var boxTitle = collectionDict.GetValueOrDefault(matchedMangaBox?.CollectionTopicId ?? "")?.Topic ?? "Unknown";

                return new UserBoxGetAllDto
                {
                    Id = box.Id,
                    UserId = box.UserId,
                    BoxId = box.BoxId,
                    Quantity = box.Quantity,
                    BoxTitle = boxTitle,
                    UpdatedAt = box.UpdatedAt
                };
            }).ToList();
        }

        public async Task<ProductResultDto> OpenMysteryBoxAsync(string userBoxId, string userId)
        {
            var userBox = await _userBoxCollection
                .Find(b => b.Id == userBoxId && b.UserId == userId)
                .FirstOrDefaultAsync();
            if (userBox == null) throw new Exception("UserBox not found");
            if (userBox.Quantity <= 0) throw new Exception("You have no boxes left to open.");

            var mangaBox = await _mangaBoxRepository.GetByIdWithDetailsAsync(userBox.BoxId);
            if (mangaBox == null || mangaBox.Products == null || !mangaBox.Products.Any()) throw new Exception("MangaBox not found or has no products.");

            var selectedProduct = RandomByChance(mangaBox.Products);
            if (selectedProduct == null) throw new Exception("Failed to select product");

            var collection = await _collectionCollection
                .Find(c => c.Topic == mangaBox.CollectionTopic)
                .FirstOrDefaultAsync();
            if (collection == null) throw new Exception("Collection not found");

            var userCollection = await _userCollectionCollection
                .Find(c => c.UserId == userId && c.CollectionId == collection.Id)
                .FirstOrDefaultAsync();
            if (userCollection == null)
            {
                userCollection = new UserCollection
                {
                    UserId = userId,
                    CollectionId = collection.Id,
                };
                await _userCollectionCollection.InsertOneAsync(userCollection);
            }

            var userProduct = await _userProductCollection
                .Find(p => p.CollectionId == userCollection.Id && p.ProductId == selectedProduct.ProductId)
                .FirstOrDefaultAsync();
            if (userProduct != null)
            {
                userProduct.Quantity++;
                await _userProductCollection.ReplaceOneAsync(p => p.Id == userProduct.Id, userProduct);
            }
            else
            {
                userProduct = new UserProduct
                {
                    CollectionId = userCollection.Id,
                    ProductId = selectedProduct.ProductId,
                    Quantity = 1,
                    CollectedAt = DateTime.UtcNow,
                    CollectorId = userId,
                };
                await _userProductCollection.InsertOneAsync(userProduct);
            }

            userBox.Quantity--;
            userBox.UpdatedAt = DateTime.UtcNow;
            await _userBoxCollection.ReplaceOneAsync(b => b.Id == userBox.Id, userBox);

            return new ProductResultDto
            {
                ProductId = selectedProduct.ProductId,
                ProductName = selectedProduct.ProductName,
                UrlImage = selectedProduct.UrlImage,
                Rarity = selectedProduct.RarityName
            };
        }

        private ProductInBoxDto RandomByChance(List<ProductInBoxDto> products)
        {
            if (products == null || products.Count == 0) throw new ArgumentException("No products to choose from");

            var rand = new Random();
            var grouped = products
                .GroupBy(p => p.Chance)
                .OrderBy(g => g.Key)
                .ToList();
            var ranges = new List<(double start, double end, List<ProductInBoxDto> items)>();
            double cumulative = 0;

            foreach (var group in grouped)
            {
                double rangeSize = group.Key * group.Count();
                double start = cumulative;
                double end = cumulative + rangeSize;

                ranges.Add((start, end, group.ToList()));
                cumulative = end;
            }

            double roll = rand.NextDouble();

            foreach (var (start, end, items) in ranges)
            {
                if (roll >= start && roll < end) return items[rand.Next(items.Count)];

            }

            return products.Last();
        }

    }
}
