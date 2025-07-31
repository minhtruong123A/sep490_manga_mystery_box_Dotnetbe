using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string RarityId { get; set; }
        public string Description { get; set; }
        public IFormFile? UrlImage { get; set; }
        public string CollectionId { get; set; }
    }
}
