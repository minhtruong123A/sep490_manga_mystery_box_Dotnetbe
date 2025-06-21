namespace DataAccessLayers.Interface
{
    public interface IPayOSRepository
    {
        Task<bool> RechargeWalletAsync(string orderCode, int amount);
    }
}