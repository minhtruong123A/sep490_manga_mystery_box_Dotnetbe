using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class MysteryBoxService(IUnitOfWork unitOfWork) : IMysteryBoxService
{
    public async Task<List<string>> GetAllUniqueImageUrlsAsync()
    {
        var boxes = await unitOfWork.MysteryBoxRepository.GetAllAsync();
        var uniqueUrls = boxes
            .Where(box => !string.IsNullOrEmpty(box.UrlImage))
            .Select(box => box.UrlImage.Trim())
            .Distinct()
            .ToList();

        return uniqueUrls;
    }

    public async Task<string> GetImageUrlsByCollectionIdAsync(string collectionId)
    {
        var mangaBoxes = await unitOfWork.MangaBoxRepository.FindAsync(x => x.CollectionTopicId == collectionId);
        if (mangaBoxes == null || !mangaBoxes.Any()) return null;
        var mysteryBoxIds = mangaBoxes
            .Select(m => m.MysteryBoxId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();
        if (!mysteryBoxIds.Any()) return null;

        var mysteryBoxes = await unitOfWork.MysteryBoxRepository.FindAsync(x => mysteryBoxIds.Contains(x.Id));
        var imageUrl = mysteryBoxes
            .Where(box => !string.IsNullOrWhiteSpace(box.UrlImage))
            .Select(box => box.UrlImage.Trim())
            .Distinct().FirstOrDefault();

        return string.IsNullOrEmpty(imageUrl) ? null : imageUrl;
    }
}