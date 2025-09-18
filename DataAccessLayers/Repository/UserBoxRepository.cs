using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserBox;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class UserBoxRepository(
    MongoDbContext context,
    IMangaBoxRepository mangaBoxRepository,
    IUserAchievementRepository userAchievementRepository)
    : GenericRepository<UserBox>(context.GetCollection<UserBox>("UserBox")), IUserBoxRepository
{
    private readonly IMongoCollection<Collection> _collectionCollection = context.GetCollection<Collection>("Collection");
    private readonly IMongoCollection<MangaBox> _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");

    private readonly IMongoCollection<MysteryBox> _mysteryBoxCollection = context.GetCollection<MysteryBox>("MysteryBox");
    private readonly IMongoCollection<UserBox> _userBoxCollection = context.GetCollection<UserBox>("UserBox");
    private readonly IMongoCollection<UserCollection> _userCollectionCollection = context.GetCollection<UserCollection>("UserCollection");
    private readonly IMongoCollection<UserProduct> _userProductCollection = context.GetCollection<UserProduct>("User_Product");

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
        var result = new List<UserBoxGetAllDto>();

        foreach (var box in userBox)
        {
            var matchedMangaBox = mangaBoxDict.GetValueOrDefault(box.BoxId);
            var mysteryboxBox = await _mysteryBoxCollection
                .Find(c => c.Id == matchedMangaBox.MysteryBoxId)
                .FirstOrDefaultAsync();
            //var boxTitle = collectionDict.GetValueOrDefault(matchedMangaBox?.CollectionTopicId ?? "")?.Topic ?? "Unknown";
            result.Add(new UserBoxGetAllDto
            {
                Id = box.Id,
                UserId = box.UserId,
                BoxId = box.BoxId,
                Quantity = box.Quantity,
                UrlImage = mysteryboxBox.UrlImage ?? "unknown",
                BoxTitle = mysteryboxBox.Name ?? "unknown",
                UpdatedAt = box.UpdatedAt
            });
        }

        return result;
    }

    public async Task<ProductResultDto> OpenMysteryBoxAsync(string userBoxId, string userId)
    {
        var userBox = await _userBoxCollection
            .Find(b => b.Id == userBoxId && b.UserId == userId)
            .FirstOrDefaultAsync();
        if (userBox == null) throw new Exception("UserBox not found");
        if (userBox.Quantity <= 0) throw new Exception("You have no boxes left to open.");

        var mangaBox = await mangaBoxRepository.GetByIdWithDetailsAsync(userBox.BoxId);
        Console.WriteLine("fsdfaadsafas" + mangaBox);
        var gogo = mangaBox.Products;
        if (mangaBox == null || mangaBox.Products == null || !mangaBox.Products.Any())
            throw new Exception("MangaBox not found or has no products.");

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
                CollectionId = collection.Id
            };
            await _userCollectionCollection.InsertOneAsync(userCollection);
        }

        var userProduct = await _userProductCollection
            .Find(p => p.CollectionId == userCollection.Id && p.ProductId == selectedProduct.ProductId)
            .FirstOrDefaultAsync();
        if (userProduct != null)
        {
            userProduct.Quantity++;
            userProduct.isQuantityUpdateInc = true;
            userProduct.UpdateAt = DateTime.UtcNow;
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
                UpdateAt = DateTime.UtcNow,
                isQuantityUpdateInc = true
            };
            await _userProductCollection.InsertOneAsync(userProduct);
        }

        userBox.Quantity--;
        userBox.UpdatedAt = DateTime.UtcNow;
        await _userBoxCollection.ReplaceOneAsync(b => b.Id == userBox.Id, userBox);

        await userAchievementRepository.CheckAchievement(userId);
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
        if (products == null || !products.Any()) throw new ArgumentException("No products to choose from");

        var rand = new Random();
        var rarityChances = products
            .GroupBy(p => p.RarityName)
            .Select(group => new
            {
                RarityName = group.Key, group.First().Chance
            })
            .OrderBy(r => r.Chance)
            .ToList();

        var rarityRoll = rand.NextDouble();
        double cumulative = 0;
        string selectedRarityName = null;

        foreach (var rarity in rarityChances)
        {
            cumulative += rarity.Chance;
            if (rarityRoll < cumulative)
            {
                selectedRarityName = rarity.RarityName;
                break;
            }
        }

        if (selectedRarityName == null) selectedRarityName = rarityChances.Last().RarityName;

        var itemsInSelectedRarity = products
            .Where(p => p.RarityName == selectedRarityName)
            .ToList();
        var itemIndex = rand.Next(itemsInSelectedRarity.Count);

        return itemsInSelectedRarity[itemIndex];
    }
}