using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class ProductWithRarityForModeratorDto
    {
        
            public string ProductId { get; set; }
            public string Name { get; set; }
            public string UrlImage { get; set; }
            public string Description { get; set; }
            public string RarityName { get; set; }
            public bool is_Block { get; set; }
        
    }
}
