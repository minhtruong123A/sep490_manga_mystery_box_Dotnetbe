using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DataAccessLayers.Repository;

public class MangaBoxRepository(MongoDbContext context)
    : GenericRepository<MangaBox>(context.GetCollection<MangaBox>("MangaBox")), IMangaBoxRepository
{
    private readonly IMongoCollection<Collection> _collectionCollection = context.GetCollection<Collection>("Collection");
    private readonly IMongoCollection<MangaBox> _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
    private readonly IMongoCollection<MysteryBox> _mysteryBoxCollection = context.GetCollection<MysteryBox>("MysteryBox");
    private readonly IMongoCollection<Product> _productCollection = context.GetCollection<Product>("Product");
    private readonly IMongoCollection<ProductInMangaBox> _productInMangaBoxCollection = context.GetCollection<ProductInMangaBox>("ProductInMangaBox");
    private readonly IMongoCollection<Rarity> _rarityCollection = context.GetCollection<Rarity>("Rarity");

    //getallwwithdetail
    //public async Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync()
    //{
    //    var results = await _mangaBoxCollection
    //        .WithBson()
    //        .RunAggregateWithLookups(
    //            MangaBoxPipelineBuilder.BuildAllPipeline,
    //            x => new MangaBoxGetAllDto
    //            {
    //                Id = x.GetValue("_id").ToString(),
    //                MysteryBoxName = x["MysteryBox"]["Name"].AsString,
    //                MysteryBoxPrice = x["MysteryBox"]["Price"].ToInt32(),
    //                UrlImage = x["MysteryBox"]["UrlImage"].AsString,
    //                CollectionTopic = x["Collection"]["Topic"].AsString
    //            });

    //    return results;
    //}
    public async Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync()
    {
        var mangaBoxes = await _mangaBoxCollection.AsQueryable().ToListAsync();
        if (!mangaBoxes.Any()) return new List<MangaBoxGetAllDto>();
        var mysteryBoxIds = mangaBoxes.Select(c => c.MysteryBoxId.Trim()).ToHashSet();
        var collectionTopicIds = mangaBoxes.Select(c => c.CollectionTopicId.Trim()).ToHashSet();
        var mysteryBoxTask = _mysteryBoxCollection.AsQueryable().Where(c => mysteryBoxIds.Contains(c.Id.ToString()))
            .ToListAsync();
        var collectionTask = _collectionCollection.AsQueryable()
            .Where(c => collectionTopicIds.Contains(c.Id.ToString())).ToListAsync();
        await Task.WhenAll(mysteryBoxTask, collectionTask);

        var mysteryBoxList = mysteryBoxTask.Result;
        var collectionList = collectionTask.Result;

        return mangaBoxes.Select(mangabox =>
        {
            var mysteryBox = mysteryBoxList.FirstOrDefault(c => c.Id.ToString() == mangabox.MysteryBoxId.Trim());
            var collection = collectionList.FirstOrDefault(c => c.Id.ToString() == mangabox.CollectionTopicId.Trim());

            return new MangaBoxGetAllDto
            {
                Id = mangabox.Id.ToString(),
                MysteryBoxName = mysteryBox?.Name ?? "Unknown",
                MysteryBoxPrice = mysteryBox?.Price ?? 0,
                UrlImage = mysteryBox?.UrlImage ?? "Unknonwn",
                CollectionTopic = collection?.Topic ?? "Unknown",
                CreatedAt = mangabox?.CreatedAt ?? null,
                Quantity = mangabox.Quantity,
                End_time = mangabox.End_time,
                Start_time = mangabox.Start_time,
                Status = mangabox.Status
            };
        }).ToList();
    }

    //getallwithdetailbyid
    //public async Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id)
    //{
    //    var objectId = ObjectId.Parse(id);

    //    return await _mangaBoxCollection
    //        .WithBson()
    //        .RunAggregateWithLookupsSingle(
    //            pipeline => MangaBoxPipelineBuilder.BuildDetailPipeline(pipeline, objectId),
    //            x => new MangaBoxDetailDto
    //            {
    //                Id = x.GetValue("_id").ToString(),
    //                Status = x.GetValue("Status").ToInt32(),
    //                MysteryBoxName = x["MysteryBox"]["Name"].AsString,
    //                MysteryBoxDescription = x["MysteryBox"]["Description"].AsString,
    //                MysteryBoxPrice = x["MysteryBox"]["Price"].ToInt32(),
    //                UrlImage = x["MysteryBox"]["UrlImage"].AsString,
    //                CollectionTopic = x["Collection"]["Topic"].AsString
    //            });
    //}


    public async Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id)
    {
        var mangaBox = await _mangaBoxCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == id);
        if (mangaBox is null) return null;

        var mysteryBoxTask = _mysteryBoxCollection.AsQueryable()
            .FirstOrDefaultAsync(c => c.Id.ToString() == mangaBox.MysteryBoxId.Trim());
        var collectionTask = _collectionCollection.AsQueryable()
            .FirstOrDefaultAsync(c => c.Id.ToString() == mangaBox.CollectionTopicId.Trim());

        var productInMangaBoxes =
            await _productInMangaBoxCollection.AsQueryable().Where(p => p.MangaBoxId == id).ToListAsync();
        var productIds = productInMangaBoxes.Select(p => p.ProductId).Distinct().ToList();
        var products = await _productCollection.AsQueryable().Where(p => productIds.Contains(p.Id)).ToListAsync();
        var rarityIds = products.Select(p => p.RarityId).Distinct().ToList();
        var rarities = await _rarityCollection.AsQueryable().Where(r => rarityIds.Contains(r.Id)).ToListAsync();
        var productDtos = productInMangaBoxes.Select(p =>
        {
            var product = products.FirstOrDefault(prod => prod.Id == p.ProductId);
            var rarity = rarities.FirstOrDefault(r => r.Id == product?.RarityId);

            return new ProductInBoxDto
            {
                ProductId = product?.Id ?? "",
                ProductName = product?.Name ?? "Unknown",
                UrlImage = product?.UrlImage ?? "",
                RarityName = rarity?.Name ?? "Unknown",
                Chance = p.Chance
            };
        }).ToList();
        await Task.WhenAll(mysteryBoxTask, collectionTask);

        var mysteryBox = mysteryBoxTask.Result;
        var collection = collectionTask.Result;

        return new MangaBoxDetailDto
        {
            Id = mangaBox.Id,
            Status = mangaBox.Status,
            MysteryBoxName = mysteryBox?.Name ?? "Unknown",
            MysteryBoxDescription = mysteryBox?.Description ?? "No description",
            MysteryBoxPrice = mysteryBox?.Price ?? 0,
            UrlImage = mysteryBox?.UrlImage,
            CollectionTopic = collection?.Topic ?? "Unknown",
            TotalProduct = mysteryBox.TotalProduct,
            Quantity = mangaBox.Quantity,
            Start_time = mangaBox.Start_time,
            End_time = mangaBox.End_time,
            Products = productDtos
        };
    }

    public async Task<MysteryBox?> FindMysteryBoxByUrlImageAsync(string urlImage)
    {
        return await _mysteryBoxCollection.Find(m => m.UrlImage == urlImage).FirstOrDefaultAsync();
    }
}