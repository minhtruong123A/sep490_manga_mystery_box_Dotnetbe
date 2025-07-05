using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.UserBox
{
    public class UserBoxGetAllDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string BoxId { get; set; }
        public int Quantity { get; set; }
        public string BoxTitle { get; set; }
        public string UrlImage { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
