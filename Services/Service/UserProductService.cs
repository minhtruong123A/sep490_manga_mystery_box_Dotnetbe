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
    }
}
