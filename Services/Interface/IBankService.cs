using BusinessObjects;
using BusinessObjects.Dtos.Bank;

namespace Services.Interface;

public interface IBankService
{
    Task<List<Bank>> GetAllAsync();
    Task<string> CreateAsync(List<BankCreateDto> dto);
}