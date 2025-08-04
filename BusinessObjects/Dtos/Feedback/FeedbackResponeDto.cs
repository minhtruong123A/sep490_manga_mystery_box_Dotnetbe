using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Feedback
{
    public class FeedbackResponeDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public float Rating { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
