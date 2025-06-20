using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.PayOS
{
    public class CreatePaymentRequest
    {
        public List<ItemRequest> Items { get; set; }
    }
}
