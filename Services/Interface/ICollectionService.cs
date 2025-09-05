using BusinessObjects;

namespace Services.Interface;

public interface ICollectionService
{
    Task<List<Collection>> GetAllAsync();
    Task<int> CreateCollectionAsync(string topic);
}