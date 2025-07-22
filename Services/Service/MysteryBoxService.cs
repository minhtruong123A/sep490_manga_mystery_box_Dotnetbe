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
    }

}
