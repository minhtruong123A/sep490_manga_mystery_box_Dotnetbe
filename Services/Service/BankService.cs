using BusinessObjects.Dtos.MangaBox;
using BusinessObjects;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Interface;

namespace Services.Service
{
    public class BankService : IBankService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BankService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Bank>> GetAllAsync()
        {
            var result = await _unitOfWork.BankRepository.GetAllAsync();
            return result.ToList();
        }
    }
}
