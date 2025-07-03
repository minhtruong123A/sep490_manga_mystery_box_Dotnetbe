using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class BuySellProductRequestDto
    {
        public string SellProductId { get; set; }
        public int Quantity { get; set; }
    }
}
