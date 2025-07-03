using BusinessObjects.Mongodb;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository
{
    public class MysteryBoxRepository : GenericRepository<MysteryBox>, IMysteryBoxRepository
    {
        public MysteryBoxRepository(MongoDbContext context) : base(context.GetCollection<MysteryBox>("MysteryBox"))
        {
        }
    }
}
