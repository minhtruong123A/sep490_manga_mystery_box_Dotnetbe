using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
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
    public class UserProductRepository : GenericRepository<UserProduct>, IUserProductRepository
    {

        private readonly IMongoCollection<UserProduct> _usersProduct;

        public UserProductRepository(MongoDbContext context) : base(context.GetCollection<UserProduct>("User_Product"))
        {
            _usersProduct = context.GetCollection<UserProduct>("User_Product");
        }

    }
}
