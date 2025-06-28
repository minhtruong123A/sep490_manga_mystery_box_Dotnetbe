using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
using DataAccessLayers.Interface;
using DataAccessLayers.Repository;
using DataAccessLayers.UnitOfWork;
using MongoDB.Driver;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class UserProductService : IUserProductService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        private readonly IMapper _mapper;
        public UserProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uniUnitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string id, string collectionId) => await _uniUnitOfWork.UserProductRepository.GetAllWithDetailsAsync(id, collectionId);
        public async Task AddCardsToCollectionAsync(string userId, string collectionId, List<string> productIds)
        {
            foreach (var productId in productIds)
            {
                var existing = await _uniUnitOfWork.UserProductRepository.FindOneAsync(up =>
                    up.CollectorId == userId &&
                    up.CollectionId == collectionId &&
                    up.ProductId == productId);

                if (existing != null)
                {
                  
                    var update = Builders<UserProduct>.Update
                                    .Inc(up => up.Quantity, 1)
                                    .Set(up => up.CollectedAt, DateTime.UtcNow);

                    await _uniUnitOfWork.UserProductRepository.UpdateOneAsync(existing.Id, update);
                }
                else
                {
                    // Nếu chưa có thì tạo mới
                    var newItem = new UserProduct
                    {
                        CollectorId = userId,
                        CollectionId = collectionId,
                        ProductId = productId,
                        Quantity = 1,
                        CollectedAt = DateTime.UtcNow
                    };

                    await _uniUnitOfWork.UserProductRepository.AddAsync(newItem);
                }
            }
        }

    }
}
