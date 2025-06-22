using BusinessObjects.Dtos.MangaBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Cart
{
    public class CartBoxDto
    {
        public string MangaBoxId { get; set; } = null!;
        public MangaBoxDetailDto Box { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
