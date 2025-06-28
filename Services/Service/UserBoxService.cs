using BusinessObjects.Dtos.UserBox;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class UserBoxService : IUserBoxService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        public UserBoxService(IUnitOfWork uniUnitOfWork)
        {
            _uniUnitOfWork = uniUnitOfWork;
        }

        public async Task<List<UserBoxGetAllDto>> GetAllWithDetailsAsync(string userId) => await _uniUnitOfWork.UserBoxRepository.GetAllWithDetailsAsync(userId);
    }
}
