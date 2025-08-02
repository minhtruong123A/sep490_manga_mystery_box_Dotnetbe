using BusinessObjects;
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
    public class RarityRepository : GenericRepository<Rarity>, IRarityRepository
    {
        private readonly IMongoCollection<Rarity> _rarityCollection;
        public RarityRepository(MongoDbContext context) : base(context.GetCollection<Rarity>("Rarity"))
        {
            _rarityCollection = context.GetCollection<Rarity>("Rarity");
        }

        public async Task<string?> GetRarityByNameAsync(string name)
        {
            var rarity = await _rarityCollection.Find(x => x.Name == name).FirstOrDefaultAsync();
            if (rarity == null) throw new Exception("Rarity not exist");
            return rarity.Id.ToString(); 
        }

    }
}
