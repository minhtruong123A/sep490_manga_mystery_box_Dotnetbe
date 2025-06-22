using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Cart
{
    public class CartViewDto
    {
        public List<CartProductDto> Products { get; set; } = new();
        public List<CartBoxDto> Boxes { get; set; } = new();
    }
}
