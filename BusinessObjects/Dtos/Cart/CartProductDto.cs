using BusinessObjects.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Cart
{
    public class CartProductDto
    {
        public string CartProductId { get; set; } = null!;
        public string SellProductId { get; set; } = null!;
        public SellProductDetailDto Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
