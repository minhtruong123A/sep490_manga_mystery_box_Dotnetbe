using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Comment
{
    public class CommentCreateDto
    {
        public string SellProductId { get; set; }
        public string Content { get; set; }
    }
}
