using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserCollection;
using BusinessObjects.Dtos.UserProduct;
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
        public async Task AddCardsToCollectionAsync(string userId, string userCollectionId, List<UserProductAddToCollectionDto> dto)
        {
            foreach (var product in dto)
            {
                var existing = await _uniUnitOfWork.UserProductRepository.FindOneAsync(up =>
                    up.CollectorId == userId &&
                    up.CollectionId ==product.UserCollectionId &&
                    up.Id == product.UserProductId);

                if (existing != null)
                {
                  if(existing.Quantity >= 1)
                    {
                        var update = Builders<UserProduct>.Update
                                    .Inc(up => up.Quantity, -1);
                        await _uniUnitOfWork.UserProductRepository.UpdateOneAsync(existing.Id, update);
                        await _uniUnitOfWork.SaveChangesAsync();
                    }
                    if (existing.Quantity == 0) throw new Exception("You no longer own this product.");
                    var newItem = new UserProduct
                    {
                        CollectorId = userId,
                        CollectionId = userCollectionId,
                        ProductId = existing.ProductId,
                        Quantity = 1,
                        CollectedAt = DateTime.UtcNow
                    };

                    await _uniUnitOfWork.UserProductRepository.AddAsync(newItem);
                }
            }
        }

    }
}
