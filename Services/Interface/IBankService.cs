using BusinessObjects;
using BusinessObjects.Dtos.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IBankService
    {
        Task<List<Bank>> GetAllAsync();
        Task<string> CreateAsync(List<BankCreateDto> dto);
    }
}
