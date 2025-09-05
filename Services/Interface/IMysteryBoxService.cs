namespace Services.Interface;

public interface IMysteryBoxService
{
    Task<List<string>> GetAllUniqueImageUrlsAsync();
    Task<string> GetImageUrlsByCollectionIdAsync(string collectionId);
}