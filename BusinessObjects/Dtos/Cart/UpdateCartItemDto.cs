using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Cart
{
    public class UpdateCartItemDto
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }
}
