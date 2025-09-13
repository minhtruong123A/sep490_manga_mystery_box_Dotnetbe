using BusinessObjects;

namespace Services.Interface;

public interface IUseDigitalWalletService
{
    Task<UseDigitalWallet?> GetWalletByIdAsync(string id);
    Task<string> UpdateWalletWithTransactionAsync(string userId, int amount);
}