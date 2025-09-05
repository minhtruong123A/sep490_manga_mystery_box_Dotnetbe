using BusinessObjects;
using BusinessObjects.Dtos.Bank;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class BankService(IUnitOfWork unitOfWork) : IBankService
{
    public async Task<List<Bank>> GetAllAsync()
    {
        var result = await unitOfWork.BankRepository.GetAllAsync();
        return result.ToList();
    }

    public async Task<string> CreateAsync(List<BankCreateDto> dto)
    {
        string respone;
        foreach (var item in dto)
        {
            var bank = new Bank();
            bank = await unitOfWork.BankRepository.GetBankByAbbreviation(item.Abbreviation);
            if (bank == null)
            {
                bank.Abbreviation = item.Abbreviation;
                bank.Name = item.Name;
                await unitOfWork.BankRepository.AddAsync(bank);
            }
        }

        return respone = "Add new bank successfully";
    }
}