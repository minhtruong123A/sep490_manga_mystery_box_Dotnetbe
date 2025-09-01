using BusinessObjects.Dtos.Product;
using BusinessObjects;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Dtos.UserCollection;

namespace Services.Service
{
    public class ProductFavoriteService : IProductFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductFavoriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<UserCollectionGetAllDto>> GetFavoriteListWithDetailsAsync(string userId) => await _unitOfWork.ProductFavoriteRepository.GetFavoriteListWithDetailsAsync(userId);
        public async Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string userId)=> await _unitOfWork.ProductFavoriteRepository.GetAllWithDetailsAsync(userId);

        public async Task<bool> CreateAsync(string userId, string userProductId)
        {
            var favorites = await _unitOfWork.ProductFavoriteRepository.GetAllAsync();
            var exist = favorites.Where(x=> x.User_Id.Equals(userId) && x.User_productId.Equals(userProductId)).FirstOrDefault();
            var count = favorites.Where(x => x.User_Id.Equals(userId)).Count();
            if (exist != null) return false;
            //if (count >= 6) return false;
            var newFavorite = new ProductFavorite {User_Id = userId, User_productId = userProductId};
            await _unitOfWork.ProductFavoriteRepository.AddAsync(newFavorite);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string favoriteId)
        {
            await _unitOfWork.ProductFavoriteRepository.DeleteAsync(favoriteId);
            return true;
        }   
    }
}
