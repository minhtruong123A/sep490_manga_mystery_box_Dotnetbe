using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Report
{
    public class ReportResponeDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string SellProductId { get; set; }
        public string ProductName { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
