using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Comment
{
    public class CommentRatingCreateDto
    {
        public string SellProductId { get; set; }
        public float Rating { get; set; }
    }
}
