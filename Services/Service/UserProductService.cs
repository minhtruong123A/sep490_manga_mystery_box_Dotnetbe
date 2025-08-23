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
        public async Task<bool> CheckedUpdateQuantityAsync(string userProductId)
        {
            var userProduct = await _uniUnitOfWork.UserProductRepository.GetByIdAsync(userProductId);
            if (userProduct == null) throw new Exception("User product not found!");

            userProduct.isQuantityUpdateInc = false;
            await _uniUnitOfWork.UserProductRepository.UpdateAsync(userProductId, userProduct);
            await _uniUnitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
