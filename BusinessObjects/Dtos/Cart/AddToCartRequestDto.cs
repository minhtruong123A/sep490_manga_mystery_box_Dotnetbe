using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Cart
{
    public class AddToCartRequestDto
    {
        public string? SellProductId { get; set; }
        public string? MangaBoxId { get; set; }
        [DefaultValue(1)]
        public int? Quantity { get; set; }
    }
}
