using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.PayOS
{
    public class PayOSWebhookRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("data")]
        public PayOSWebhookData Data { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}
