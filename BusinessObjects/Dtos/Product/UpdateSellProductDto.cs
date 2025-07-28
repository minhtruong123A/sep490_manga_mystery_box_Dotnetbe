using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class UpdateSellProductDto
    {
        public string Id { get; set; }
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 300 characters.")]
        public string Description { get; set; }
        public int Price { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
