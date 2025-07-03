using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class ProductResultDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string UrlImage { get; set; }
        public string Rarity { get; set; }
    }
}
