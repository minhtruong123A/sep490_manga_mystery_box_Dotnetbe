using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Report
{
    public class ReportCreateDto
    {
        public string? SellProductId { get; set; }
        public string? SellerId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

    }
}
