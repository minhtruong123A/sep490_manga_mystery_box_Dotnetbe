using BusinessObjects;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class CollectionService : ICollectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CollectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Collection>> GetAllAsync()
        {
            var collections = await _unitOfWork.CollectionRepository.GetAllAsync();
            return collections.ToList();
        }

        public async Task<int> CreateCollectionAsync(string topic)
        {
            var collections = await _unitOfWork.CollectionRepository.GetAllAsync();
            var exist = collections.FirstOrDefault(x => x.Topic == topic);
            if (exist == null)
            {
                var newCollection = new Collection();
                newCollection.Topic = topic;
                newCollection.IsSystem = true;
                await _unitOfWork.CollectionRepository.AddAsync(newCollection);
                await _unitOfWork.SaveChangesAsync();
                return 1;
            }
            return 0;
        }
    }
}
