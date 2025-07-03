using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUseDigitalWalletService
    {
        Task<UseDigitalWallet?> GetWalletByIdAsync(string id);
        Task<string> UpdateWalletWithTransactionAsync(string userId, int amount);
    }
}