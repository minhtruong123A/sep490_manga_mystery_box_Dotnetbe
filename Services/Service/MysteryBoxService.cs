using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class MysteryBoxService : IMysteryBoxService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MysteryBoxService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<string>> GetAllUniqueImageUrlsAsync()
        {
            var boxes = await _unitOfWork.MysteryBoxRepository.GetAllAsync();
            var uniqueUrls = boxes
                .Where(box => !string.IsNullOrEmpty(box.UrlImage))
                .Select(box => box.UrlImage.Trim())
                .Distinct()
                .ToList();

            return uniqueUrls;
        }

        public async Task<string> GetImageUrlsByCollectionIdAsync(string collectionId)
        {
            var mangaBoxes = await _unitOfWork.MangaBoxRepository.FindAsync(x => x.CollectionTopicId == collectionId);
            if (mangaBoxes == null || !mangaBoxes.Any()) return null;
            var mysteryBoxIds = mangaBoxes
                .Select(m => m.MysteryBoxId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();
            if (!mysteryBoxIds.Any())  return null;

            var mysteryBoxes = await _unitOfWork.MysteryBoxRepository.FindAsync(x => mysteryBoxIds.Contains(x.Id));
            var imageUrls = mysteryBoxes
                .Where(box => !string.IsNullOrWhiteSpace(box.UrlImage))
                .Select(box => box.UrlImage.Trim())
                .Distinct()
                .ToString();

            return imageUrls.Any() ? imageUrls : null;
        }
    }

}
