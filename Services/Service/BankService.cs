using BusinessObjects;
using BusinessObjects.Dtos.Bank;
using BusinessObjects.Dtos.MangaBox;
using DataAccessLayers.Interface;
using Microsoft.Extensions.Configuration;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> CreateAsync(List<BankCreateDto> dto)
        {
            string respone;
            foreach (var item in dto)
            {

                Bank bank = new Bank();
                bank = await _unitOfWork.BankRepository.GetBankByAbbreviation(item.Abbreviation);
                if (bank == null)
                {
                    bank.Abbreviation = item.Abbreviation;
                    bank.Name = item.Name;
                    await _unitOfWork.BankRepository.AddAsync(bank);
                }
            }
            return respone = "Add new bank successfully";
        }
    }
}
