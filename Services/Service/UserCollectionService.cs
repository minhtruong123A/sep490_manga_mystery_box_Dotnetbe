using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Collection;
using BusinessObjects.Dtos.UserCollection;
using DataAccessLayers.Interface;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class UserCollectionService : IUserCollectionService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        private readonly IMapper _mapper;

        public UserCollectionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uniUnitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateUserCollectionAsync(UserCollection collection) => await _uniUnitOfWork.UserCollectionRepository.AddAsync(collection);
        public async Task<List<UserCollectionGetAllDto>> GetAllWithDetailsAsync(string id) => await _uniUnitOfWork.UserCollectionRepository.GetAllWithDetailsAsync(id);
        public async Task CreateUserCollectionByUserAsync(string userId, CollectionCreateByUserDto dto)
        {
            var newCollection = new Collection();
            newCollection.Topic = dto.Topic;
            newCollection.IsSystem = false;
            await _uniUnitOfWork.CollectionRepository.AddAsync(newCollection);

            var collection = await _uniUnitOfWork.CollectionRepository.FindOneAsync(x => x.Topic.Equals(dto.Topic) && x.IsSystem == false);
            var newUserCollection = new UserCollection();
            newUserCollection.UserId = userId;
            newUserCollection.CollectionId = collection.Id;
            await _uniUnitOfWork.UserCollectionRepository.AddAsync(newUserCollection);     
        }
        

    }
}
