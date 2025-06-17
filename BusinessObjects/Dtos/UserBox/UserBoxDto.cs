using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.UserMangaBox
{
    public class UserBoxDto
    {
        public string UserId { get; set; }
        public string BoxId { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
