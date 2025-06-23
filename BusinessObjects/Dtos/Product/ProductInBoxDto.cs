using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class ProductInBoxDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string UrlImage { get; set; }
        public string RarityName { get; set; }
        public double Chance { get; set; }
    }
}
