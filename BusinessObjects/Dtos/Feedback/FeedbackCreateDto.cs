using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Feedback
{
    public class FeedbackCreateDto
    {
        public string Exchange_infoId { get; set; }
        public string Content { get; set; }
        public float Rating { get; set; }
    }
}
