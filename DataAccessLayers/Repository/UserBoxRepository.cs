using BusinessObjects;
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
        private readonly IMongoCollection<Collection> _collectionCollection; 
        public UserBoxRepository(MongoDbContext context) : base(context.GetCollection<UserBox>("UserBox"))
        {
            _userBoxCollection = context.GetCollection<UserBox>("UserBox");
            _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
            _collectionCollection = context.GetCollection<Collection>("Collection");
        }

        public async Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId)
        {
            // 1. Lấy các UserBox của user
            var userBox = await _userBoxCollection.Find(c => c.UserId == userId).ToListAsync();
            if (!userBox.Any()) return [];

            // 2. Lấy danh sách Box tương ứng
            var boxIds = userBox.Select(c => c.BoxId).ToList();
            var boxTask = await _mangaBoxCollection.Find(c => boxIds.Contains(c.Id.ToString())).ToListAsync();
            var boxes = boxTask.ToDictionary(c => c.Id.ToString());

            // 3. Mapping sang DTO
            return userBox.Select(box =>
            {
                var matchedBox = boxes.GetValueOrDefault(box.BoxId);
                return new UserBoxGetAllDto
                {
                    UserId = box.UserId,
                    BoxId = box.BoxId,
                    Quantity = box.Quantity,
                    BoxTitle = matchedBox.Title,
                    UpdatedAt = box.UpdatedAt
                };
            }).ToList();
        }
    }
}
