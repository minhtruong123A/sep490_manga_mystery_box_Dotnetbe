using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Exchange
{
    public class ExchangeProductDetailDto
    {
        public string ProductExchangeId { get; set; }
        public int QuantityProductExchange { get; set; }
        public int Status { get; set; }
        public string Image { get; set; }
    }

}
