using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Exchange
{
    public class ExchangeGetAllWithProductDto
    {
        public string Id { get; set; }
        public string BuyerId { get; set; }
        public string ItemReciveId { get; set; }
        public string IamgeItemRecive {  get; set; }
        public string ItemGiveId { get; set; }
        public int Status { get; set; }
        public DateTime Datetime { get; set; }

        public List<ExchangeProductDetailDto> Products { get; set; }
    }
}
