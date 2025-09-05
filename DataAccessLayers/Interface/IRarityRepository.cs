using BusinessObjects;

namespace DataAccessLayers.Interface;

public interface IRarityRepository : IGenericRepository<Rarity>
{
    Task<string?> GetRarityByNameAsync(string name);
}