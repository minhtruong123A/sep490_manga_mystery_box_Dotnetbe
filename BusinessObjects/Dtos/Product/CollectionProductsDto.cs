using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class CollectionProductsDto
    {
        public string Id { get; set; }
        public string CollectionId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CollectedAt { get; set; }
        public string CollectorId { get; set; }
        public string ProductName { get; set; }
        public string UrlImage { get; set; }
    }
}
