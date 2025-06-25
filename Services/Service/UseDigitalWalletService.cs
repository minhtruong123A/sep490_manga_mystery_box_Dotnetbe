using BusinessObjects;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class UseDigitalWalletService : IUseDigitalWalletService
    {
        private readonly IUnitOfWork _uniUnitOfWork;

        public UseDigitalWalletService(IUnitOfWork unitOfWork)
        {
            _uniUnitOfWork = unitOfWork;
        }

        public async Task<UseDigitalWallet?> GetWalletByIdAsync(string id) => await _uniUnitOfWork.UseDigitalWalletRepository.GetWalletByIdAsync(id);
    }
}
