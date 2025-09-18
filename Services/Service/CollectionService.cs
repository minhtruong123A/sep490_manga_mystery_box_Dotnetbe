using BusinessObjects;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class CollectionService(IUnitOfWork unitOfWork) : ICollectionService
{
    public async Task<List<Collection>> GetAllAsync()
    {
        var collections = await unitOfWork.CollectionRepository.GetAllAsync();
        return collections.ToList();
    }

    public async Task<int> CreateCollectionAsync(string topic)
    {
        var collections = await unitOfWork.CollectionRepository.GetAllAsync();
        var exist = collections.FirstOrDefault(x => x.Topic == topic);
        if (exist != null) return 0;
        var newCollection = new Collection
        {
            Topic = topic,
            IsSystem = true
        };
        await unitOfWork.CollectionRepository.AddAsync(newCollection);
        await unitOfWork.SaveChangesAsync();
        return 1;
    }
}