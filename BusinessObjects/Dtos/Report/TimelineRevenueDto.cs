using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Report
{
    public class TimelineRevenueDto
    {
        public string Time { get; set; } // e.g. "03/08/2025" or "08/2025"
        public int Revenue { get; set; }
        public int Orders { get; set; }
        public int ProductsSold { get; set; }
    }
}
