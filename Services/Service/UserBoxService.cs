using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.UserBox;
using DataAccessLayers.Interface;
using DataAccessLayers.UnitOfWork;
using MongoDB.Driver;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

namespace Services.Service
{
    public class UserBoxService : IUserBoxService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserBoxService(IUnitOfWork uniUnitOfWork)
        {
            _unitOfWork = uniUnitOfWork;
        }

        public async Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId) => await _unitOfWork.UserBoxRepository.GetAllWithDetailsAsync(userId);
        public async Task<ProductResultDto> OpenMysteryBoxAsync(string userBoxId, string userId) => await _unitOfWork.UserBoxRepository.OpenMysteryBoxAsync(userBoxId, userId);
    }
}
