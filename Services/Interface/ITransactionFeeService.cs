using BusinessObjects;
using BusinessObjects.Dtos.TransactionFee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ITransactionFeeService
    {
        Task<List<TransactionFeeDto>> GetAllValidTransactionFeesAsync();
    }
}
