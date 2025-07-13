using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Comment
{
    public class CommentWithUsernameDto
    {
        public string Id { get; set; }
        public string SellProductId { get; set; }
        public string? ProfileImage { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Status { get; set; }
    }
}
