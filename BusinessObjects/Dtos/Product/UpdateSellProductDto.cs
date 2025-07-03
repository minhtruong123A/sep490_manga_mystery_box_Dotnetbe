using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class UpdateSellProductDto
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
