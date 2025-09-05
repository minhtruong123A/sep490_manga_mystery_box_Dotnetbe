using BusinessObjects.Dtos.TransactionFee;

namespace Services.Interface;

public interface ITransactionFeeService
{
    Task<List<TransactionFeeDto>> GetAllValidTransactionFeesAsync();
}