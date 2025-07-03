using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.UserBox
{
    public class BuyBoxRequestDto
    {
        public string MangaBoxId { get; set; }
        public int Quantity { get; set; }
    }
}
