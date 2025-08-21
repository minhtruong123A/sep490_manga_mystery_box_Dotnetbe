using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.TransactionHistory
{
    public class WithdrawLimitInfoDto
    {
        public int WithdrawCountToday { get; set; }
        public int RemainingLimit { get; set; }
        public string Message { get; set; }
    }
}
