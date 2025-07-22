using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class SellProductDetailDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserProfileImage { get; set; }
        public string Topic { get; set; }
        public string UrlImage { get; set; }
        public string RateName { get; set; } 
        public string Description { get; set; }
        public int Quantity { get; set; }
        public bool? IsSell { get; set; }

    }
}
