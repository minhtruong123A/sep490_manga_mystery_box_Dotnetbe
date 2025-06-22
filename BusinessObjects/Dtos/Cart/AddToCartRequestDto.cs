using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Cart
{
    public class AddToCartRequestDto
    {
        public string? SellProductId { get; set; }
        public string? MangaBoxId { get; set; }
    }
}
