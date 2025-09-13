using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class RarityRepository(MongoDbContext context)
    : GenericRepository<Rarity>(context.GetCollection<Rarity>("Rarity")), IRarityRepository
{
    private readonly IMongoCollection<Rarity> _rarityCollection = context.GetCollection<Rarity>("Rarity");

    public async Task<string?> GetRarityByNameAsync(string name)
    {
        var rarity = await _rarityCollection.Find(x => x.Name == name).FirstOrDefaultAsync();
        return rarity == null ? throw new Exception("Rarity not exist") : rarity.Id;
    }
}