using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.PayOS
{
    public class PayOSWebhookRequest
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public bool Success { get; set; }
        public PayOSWebhookData Data { get; set; }
        public string Signature { get; set; }
    }
}
