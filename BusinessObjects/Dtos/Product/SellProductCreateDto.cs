using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class SellProductCreateDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int ExchangeCode { get; set; }
    }
}
