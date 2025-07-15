using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Exchange
{
    public class CreateExchangeRequestDto
    {
        public string ItemReciveId { get; set; }  // SellProductId
        public ExchangeSessionDto Session { get; set; }
        public List<ExchangeProductDto> Products { get; set; }
    }

}
