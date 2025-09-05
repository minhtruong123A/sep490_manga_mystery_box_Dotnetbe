using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Collection;
using BusinessObjects.Dtos.UserCollection;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class UserCollectionService(IUnitOfWork unitOfWork, IMapper mapper) : IUserCollectionService
{
    public async Task CreateUserCollectionAsync(UserCollection collection)
    {
        await unitOfWork.UserCollectionRepository.AddAsync(collection);
    }

    public async Task<List<UserCollectionGetAllDto>> GetAllWithDetailsAsync(string id)
    {
        return await unitOfWork.UserCollectionRepository.GetAllWithDetailsAsync(id);
    }

    public async Task CreateUserCollectionByUserAsync(string userId, CollectionCreateByUserDto dto)
    {
        var newCollection = new Collection();
        newCollection.Topic = dto.Topic;
        newCollection.IsSystem = false;
        await unitOfWork.CollectionRepository.AddAsync(newCollection);

        var collection =
            await unitOfWork.CollectionRepository.FindOneAsync(x =>
                x.Topic.Equals(dto.Topic) && x.IsSystem == false);
        var newUserCollection = new UserCollection();
        newUserCollection.UserId = userId;
        newUserCollection.CollectionId = collection.Id;
        await unitOfWork.UserCollectionRepository.AddAsync(newUserCollection);
    }
}