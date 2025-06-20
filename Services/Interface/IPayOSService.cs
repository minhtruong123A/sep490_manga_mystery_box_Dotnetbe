using Net.payOS.Types;

namespace Services.Interface
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePaymentLinkAsync(
                    long orderCode,
                    int amount,
                    string description,
                    List<ItemData> items // <-- nhan tu frontend
                );
    }
}