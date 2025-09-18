using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class MysteryBoxRepository : GenericRepository<MysteryBox>, IMysteryBoxRepository
{
    public MysteryBoxRepository(MongoDbContext context) : base(context.GetCollection<MysteryBox>("MysteryBox"))
    {
    }
}